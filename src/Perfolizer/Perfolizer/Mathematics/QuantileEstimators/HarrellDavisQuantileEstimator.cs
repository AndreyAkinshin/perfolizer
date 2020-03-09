using System.Collections.Generic;
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

            int n = data.Count;
            double a = (n + 1) * quantile, b = (n + 1) * (1 - quantile);
            var distribution = new BetaDistribution(a, b);

            double result = 0;
            double betaCdfRight = 0;
            for (int j = 0; j < data.Count; j++)
            {
                double betaCdfLeft = betaCdfRight;
                betaCdfRight = distribution.Cdf((j + 1) * 1.0 / n);
                result += (betaCdfRight - betaCdfLeft) * data[j];
            }
            return result;
        }
    }
}