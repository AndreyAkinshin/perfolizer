using System;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// Piecewise-Parabolic (P^2) quantile estimator 
    ///
    /// <remarks>
    /// Based on the following paper:
    /// Jain, Raj, and Imrich Chlamtac. "The P2 algorithm for dynamic calculation of quantiles and histograms without storing observations."
    /// Communications of the ACM 28, no. 10 (1985): 1076-1085.
    /// https://doi.org/10.1145/4372.4378 <br />
    ///
    /// See also:<br />
    /// * https://aakinshin.net/posts/p2-quantile-estimator/<br />
    /// * https://aakinshin.net/posts/p2-quantile-estimator-rounding-issue/<br />
    /// * https://aakinshin.net/posts/p2-quantile-estimator-initialization/<br />
    /// * https://aakinshin.net/posts/p2-quantile-estimator-adjusting-order/
    /// </remarks>
    /// </summary>
    public class P2QuantileEstimator : ISequentialSpecificQuantileEstimator
    {
        private readonly Probability p;
        private readonly InitializationStrategy strategy;
        private readonly int[] n = new int[5];
        private readonly double[] ns = new double[5];
        private readonly double[] q = new double[5];

        public int Count { get; private set; }

        public enum InitializationStrategy
        {
            Classic, Adaptive
        }

        public P2QuantileEstimator(Probability probability, InitializationStrategy strategy = InitializationStrategy.Adaptive)
        {
            p = probability;
            this.strategy = strategy;
        }

        public void Add(double value)
        {
            if (Count < 5)
            {
                q[Count++] = value;
                if (Count == 5)
                {
                    Array.Sort(q);

                    for (int i = 0; i < 5; i++)
                        n[i] = i;
                    
                    if (strategy == InitializationStrategy.Adaptive)
                    {
                        Array.Copy(q, ns, 5);
                        n[1] = (int)Math.Round(2 * p);
                        n[2] = (int)Math.Round(4 * p);
                        n[3] = (int)Math.Round(2 + 2 * p);
                        q[1] = ns[n[1]];
                        q[2] = ns[n[2]];
                        q[3] = ns[n[3]];
                    }

                    ns[0] = 0;
                    ns[1] = 2 * p;
                    ns[2] = 4 * p;
                    ns[3] = 2 + 2 * p;
                    ns[4] = 4;
                }

                return;
            }

            int k;
            if (value < q[0])
            {
                q[0] = value;
                k = 0;
            }
            else if (value < q[1])
                k = 0;
            else if (value < q[2])
                k = 1;
            else if (value < q[3])
                k = 2;
            else if (value < q[4])
                k = 3;
            else
            {
                q[4] = value;
                k = 3;
            }

            for (int i = k + 1; i < 5; i++)
                n[i]++;
            ns[1] = Count * p / 2;
            ns[2] = Count * p;
            ns[3] = Count * (1 + p) / 2;
            ns[4] = Count;

            if (p >= 0.5)
            {
                for (int i = 1; i <= 3; i++)
                    Adjust(i);
            }
            else
            {
                for (int i = 3; i >= 1; i--)
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

        public double GetQuantile()
        {
            if (Count == 0)
                throw new EmptySequenceException();
            if (Count <= 5)
            {
                Array.Sort(q, 0, Count);
                int index = (int)Math.Round((Count - 1) * p);
                return q[index];
            }

            return q[2];
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}