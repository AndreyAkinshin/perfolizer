using System;
using JetBrains.Annotations;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// Extended Piecewise-Parabolic (P^2) quantile estimator 
    ///
    /// <remarks>
    /// Based on the following paper:
    /// Raatikainen, Kimmo EE. "Simultaneous estimation of several percentiles."
    /// Simulation 49, no. 4 (1987): 159-163.
    /// https://doi.org/10.1177/003754978704900405 <br /> 
    ///
    /// See also:<br />
    /// * https://aakinshin.net/posts/ex-p2-quantile-estimator/
    /// </remarks>
    /// </summary>
    public class ExtendedP2QuantileEstimator : ISequentialQuantileEstimator
    {
        private readonly Probability[] probabilities;
        private readonly int m, markerCount;
        private readonly int[] n;
        private readonly double[] ns;
        private readonly double[] q;

        public int Count { get; private set; }

        public ExtendedP2QuantileEstimator([NotNull] params Probability[] probabilities)
        {
            this.probabilities = probabilities;
            m = probabilities.Length;
            markerCount = 2 * m + 3;
            n = new int[markerCount];
            ns = new double[markerCount];
            q = new double[markerCount];
        }

        private void UpdateNs(int maxIndex)
        {
            // Principal markers
            ns[0] = 0;
            for (int i = 0; i < m; i++)
                ns[i * 2 + 2] = maxIndex * probabilities[i];
            ns[markerCount - 1] = maxIndex;

            // Middle markers
            ns[1] = maxIndex * probabilities[0] / 2;
            for (int i = 1; i < m; i++)
                ns[2 * i + 1] = maxIndex * (probabilities[i - 1] + probabilities[i]) / 2;
            ns[markerCount - 2] = maxIndex * (1 + probabilities[m - 1]) / 2;
        }

        public void Add(double value)
        {
            if (Count < markerCount)
            {
                q[Count++] = value;
                if (Count == markerCount)
                {
                    Array.Sort(q);

                    UpdateNs(markerCount - 1);
                    for (int i = 0; i < markerCount; i++)
                        n[i] = (int)Math.Round(ns[i]);

                    Array.Copy(q, ns, markerCount);
                    for (int i = 0; i < markerCount; i++)
                        q[i] = ns[n[i]];
                    UpdateNs(markerCount - 1);
                }

                return;
            }

            int k = -1;
            if (value < q[0])
            {
                q[0] = value;
                k = 0;
            }
            else
            {
                for (int i = 1; i < markerCount; i++)
                    if (value < q[i])
                    {
                        k = i - 1;
                        break;
                    }
                if (k == -1)
                {
                    q[markerCount - 1] = value;
                    k = markerCount - 2;
                }
            }

            for (int i = k + 1; i < markerCount; i++)
                n[i]++;
            UpdateNs(Count);

            int leftI = 1, rightI = markerCount - 2;
            while (leftI <= rightI)
            {
                int i;
                if (Math.Abs(ns[leftI] / Count - 0.5) <= Math.Abs(ns[rightI] / Count - 0.5))
                    i = leftI++;
                else
                    i = rightI--;
                Adjust(i);
            }

            Count++;
        }

        private void Adjust(int i)
        {
            double d = ns[i] - n[i];
            if (d >= 1 && n[i + 1] - n[i] > 1 || d <= -1 && n[i - 1] - n[i] < -1)
            {
                int dInt = Math.Sign(d);
                double qs = Parabolic(i, dInt);
                if (q[i - 1] < qs && qs < q[i + 1])
                    q[i] = qs;
                else
                    q[i] = Linear(i, dInt);
                n[i] += dInt;
            }
        }

        private double Parabolic(int i, double d)
        {
            return q[i] + d / (n[i + 1] - n[i - 1]) * (
                (n[i] - n[i - 1] + d) * (q[i + 1] - q[i]) / (n[i + 1] - n[i]) +
                (n[i + 1] - n[i] - d) * (q[i] - q[i - 1]) / (n[i] - n[i - 1])
            );
        }

        private double Linear(int i, int d)
        {
            return q[i] + d * (q[i + d] - q[i]) / (n[i + d] - n[i]);
        }

        public double GetQuantile(Probability p)
        {
            if (Count == 0)
                throw new EmptySequenceException();
            if (Count <= markerCount)
            {
                Array.Sort(q, 0, Count);
                int index = (int)Math.Round((Count - 1) * p);
                return q[index];
            }

            for (int i = 0; i < m; i++)
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (probabilities[i] == p)
                    return q[2 * i + 2];

            throw new InvalidOperationException($"Target quantile ({p}) wasn't requested in the constructor");
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}