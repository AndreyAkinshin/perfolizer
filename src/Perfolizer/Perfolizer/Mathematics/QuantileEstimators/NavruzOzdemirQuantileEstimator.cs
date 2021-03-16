using System.Collections.Generic;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// NO quantile estimator
    /// <remarks>
    /// Based on the following paper:
    /// Navruz, Gözde, and A. Fırat Özdemir. "A new quantile estimator with weights based on a subsampling approach."
    /// British Journal of Mathematical and Statistical Psychology 73, no. 3 (2020): 506-521.
    /// </remarks>
    /// </summary>
    public class NavruzOzdemirQuantileEstimator : BinomialBasedQuantileEstimator
    {
        public static readonly IQuantileEstimator Instance = new NavruzOzdemirQuantileEstimator();

        private NavruzOzdemirQuantileEstimator()
        {
        }

        public override string Alias => "NO";

        protected override double GetQuantile(IReadOnlyList<double> x, Probability probability, double[] b)
        {
            int n = x.Count;
            double q = probability;
            double value = 0;
            value += (b[0] * 2 * q + b[1] * q) * x[0] + b[0] * (2 - 3 * q) * x[1] - b[0] * (1 - q) * x[2];
            for (int i = 1; i <= n - 2; i++)
                value += (b[i] * (1 - q) + b[i + 1] * q) * x[i];
            value += -b[n] * q * x[n - 3] + b[n] * (3 * q - 1) * x[n - 2] + (b[n - 1] * (1 - q) + b[n] * (2 - 2 * q)) * x[n - 1];
            return value;
        }
    }
}