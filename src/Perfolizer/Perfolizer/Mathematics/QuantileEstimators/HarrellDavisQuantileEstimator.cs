using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// <remarks>
    /// Harrell, F.E. and Davis, C.E., 1982. A new distribution-free quantile estimator. Biometrika, 69(3), pp.635-640.
    /// </remarks>
    /// </summary>
    public class HarrellDavisQuantileEstimator : IQuantileEstimator
    {
        public static readonly IQuantileEstimator Instance = new HarrellDavisQuantileEstimator();

        public double GetQuantileFromSorted(IReadOnlyList<double> data, double quantile)
        {
            QuantileEstimatorHelper.CheckArguments(data, quantile);

            double result = 0;
            for (int j = 0; j < data.Count; j++)
                result += W(data, j, quantile) * data[j];
            return result;
        }

        private static double W([NotNull] IReadOnlyList<double> x, int j, double u)
        {
            int n = x.Count;
            double a = (n + 1) * u, b = (n + 1) * (1 - u);
            var distribution = new BetaDistribution(a, b);
            return distribution.Cdf((j + 1) * 1.0 / n) - distribution.Cdf(j * 1.0 / n);
        }
    }
}