using System;

namespace Perfolizer.Mathematics.Common
{
    public class Ranker
    {
        public static Ranker Instance = new();

        private Ranker()
        {
        }

        public double[] GetRanks(double[] x, double eps = 1e-9)
        {
            int n = x.Length;
            int[] index = new int[n];
            for (int i = 0; i < n; i++)
                index[i] = i;
            Array.Sort(index, (i, j) => x[i].CompareTo(x[j]));

            double[] ranks = new double[n];
            for (int i = 0; i < n; i++)
            {
                int j = i;
                while (j < n - 1 && Math.Abs(x[index[j]] - x[index[j + 1]]) < eps)
                    j++;
                if (i == j)
                    ranks[index[i]] = i + 1;
                else
                {
                    double rank = (i + j) / 2.0 + 1;
                    for (int k = i; k <= j; k++)
                        ranks[index[k]] = rank;
                    i = j;
                }
            }

            return ranks;
        }
    }
}