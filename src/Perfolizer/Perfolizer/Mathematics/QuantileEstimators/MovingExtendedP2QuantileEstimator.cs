using System;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public class MovingExtendedP2QuantileEstimator : ISequentialQuantileEstimator
    {
        private readonly ExtendedP2QuantileEstimator estimator;
        private readonly int windowSize;
        private int n;
        private readonly double[] previousWindowEstimations;

        public MovingExtendedP2QuantileEstimator([NotNull] Probability[] probabilities, int windowSize)
        {
            Assertion.Positive(nameof(windowSize), windowSize);
            this.windowSize = windowSize;
            estimator = new ExtendedP2QuantileEstimator(probabilities);
            previousWindowEstimations = new double[probabilities.Length];
        }

        public void Add(double value)
        {
            n++;
            if (n % windowSize == 0)
            {
                for (int i = 0; i < estimator.Probabilities.Length; i++)
                    previousWindowEstimations[i] = estimator.Q[2 * i + 2];
                estimator.Clear();
            }
            estimator.Add(value);
        }

        public double GetQuantile(Probability p)
        {
            if (n == 0)
                throw new EmptySequenceException();
            if (n < windowSize)
                return estimator.GetQuantile(p);

            for (int i = 0; i < estimator.Probabilities.Length; i++)
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (estimator.Probabilities[i] == p)
                {
                    double estimation1 = previousWindowEstimations[i];
                    double estimation2 = estimator.Q[2 * i + 2];
                    double w2 = (n % windowSize + 1) * 1.0 / windowSize;
                    double w1 = 1.0 - w2;
                    return w1 * estimation1 + w2 * estimation2;
                }

            throw new InvalidOperationException($"Target quantile ({p}) wasn't requested in the constructor");
        }
    }
}