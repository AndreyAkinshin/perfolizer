using System;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Distributions;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// <remarks>
    /// Harrell, F.E. and Davis, C.E., 1982. A new distribution-free quantile estimator. Biometrika, 69(3), pp.635-640.
    /// </remarks>
    /// </summary>
    public class HarrellDavisQuantileEstimator
    {
        public static readonly HarrellDavisQuantileEstimator Instance = new HarrellDavisQuantileEstimator();

        public double GetQuantile([NotNull] double[] data, double quantile)
        {
            return GetQuantiles(data, new[] {quantile}).First();
        }

        [NotNull]
        public double[] GetQuantiles([NotNull] double[] data, [NotNull] double[] quantiles)
        {
            var sortedData = data.CopyToArray();
            Array.Sort(sortedData);

            var results = new double[quantiles.Length];
            for (int i = 0; i < quantiles.Length; i++)
            {
                double u = quantiles[i];
                for (int j = 0; j < sortedData.Length; j++)
                    results[i] += W(sortedData, j, u) * sortedData[j];
            }

            return results;
        }

        private double W([NotNull] double[] x, int j, double u)
        {
            int n = x.Length;
            double a = (n + 1) * u, b = (n + 1) * (1 - u);
            var distribution = new BetaDistribution(a, b);
            return distribution.Cdf((j + 1) * 1.0 / n) - distribution.Cdf(j * 1.0 / n);
        }
    }
}