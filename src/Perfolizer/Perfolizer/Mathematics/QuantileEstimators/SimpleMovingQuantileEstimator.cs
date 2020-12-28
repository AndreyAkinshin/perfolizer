using System;
using System.Linq;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// A simple moving selector based on sorting.
    /// Memory: O(windowSize).
    /// Add complexity: O(1).
    /// GetValue complexity: O(windowSize * log(windowSize)).
    /// </summary>
    public class SimpleMovingQuantileEstimator : ISequentialQuantileEstimator
    {
        private readonly int windowSize, k;
        private readonly MovingQuantileEstimatorInitStrategy initStrategy;
        private readonly double[] values;
        private int n;

        public SimpleMovingQuantileEstimator(int windowSize, int k,
            MovingQuantileEstimatorInitStrategy initStrategy = MovingQuantileEstimatorInitStrategy.QuantileApproximation)
        {
            Assertion.Positive(nameof(windowSize), windowSize);
            Assertion.InRangeInclusive(nameof(k), k, 0, windowSize - 1);

            this.windowSize = windowSize;
            this.k = k;
            this.initStrategy = initStrategy;
            values = new double[windowSize];
        }

        public SimpleMovingQuantileEstimator(int windowSize, Probability p) : this(windowSize, (int) Math.Round((windowSize - 1) * p))
        {
        }

        public void Add(double value)
        {
            values[n++ % windowSize] = value;
        }

        public double GetQuantile()
        {
            if (n >= windowSize)
                return GetOrderStatistics(k);
            if (n == 0)
                throw new IndexOutOfRangeException("There are no any values");

            // 0 < n < windowSize
            switch (initStrategy)
            {
                case MovingQuantileEstimatorInitStrategy.QuantileApproximation:
                    return GetOrderStatistics((k * n / windowSize).Clamp(0, windowSize - 1));
                case MovingQuantileEstimatorInitStrategy.OrderStatistics:
                {
                    if (k < n)
                        return GetOrderStatistics(k);
                    throw new IndexOutOfRangeException($"Not enough values (n = {n}, k = {k})");
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(initStrategy), initStrategy,
                        $"Unknown {nameof(MovingQuantileEstimatorInitStrategy)}");
            }
        }

        private double GetOrderStatistics(int m)
        {
            Assertion.InRangeInclusive(nameof(m), m, 0, Math.Min(windowSize, n) - 1);
            double[] windowElements = n < windowSize ? values.Take(n).ToArray() : values.CopyToArray();
            Array.Sort(windowElements);
            return windowElements[m];
        }
    }
}