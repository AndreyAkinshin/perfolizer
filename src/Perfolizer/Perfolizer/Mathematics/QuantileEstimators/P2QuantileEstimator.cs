using System;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// Piecewise-Parabolic (P^2) quantile estimator 
    /// 
    /// See https://www.cse.wustl.edu/~jain/papers/ftp/psqr.pdf
    /// </summary>
    public class P2QuantileEstimator : ISequentialQuantileEstimator
    {
        private readonly Probability p;
        private readonly int[] n = new int[5];
        private readonly double[] ns = new double[5];
        private readonly double[] dns = new double[5];
        private readonly double[] q = new double[5];
        private int count;

        public P2QuantileEstimator(Probability probability)
        {
            p = probability;
        }

        public void Add(double value)
        {
            if (count < 5)
            {
                q[count++] = value;
                if (count == 5)
                {
                    Array.Sort(q);

                    for (int i = 0; i < 5; i++)
                        n[i] = i;

                    ns[0] = 0;
                    ns[1] = 2 * p;
                    ns[2] = 4 * p;
                    ns[3] = 2 + 2 * p;
                    ns[4] = 4;

                    dns[0] = 0;
                    dns[1] = p / 2;
                    dns[2] = p;
                    dns[3] = (1 + p) / 2;
                    dns[4] = 1;
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
            for (int i = 0; i < 5; i++)
                ns[i] += dns[i];

            for (int i = 1; i <= 3; i++)
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

            count++;
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
            if (count == 0)
                throw new IndexOutOfRangeException("There are no any values");
            if (count <= 5)
            {
                Array.Sort(q, 0, count);
                int index = (int) Math.Round((count - 1) * p);
                return q[index];
            }

            return q[2];
        }

        public void Clear()
        {
            count = 0;
        }
    }
}