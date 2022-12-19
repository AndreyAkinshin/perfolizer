using System.Collections.Generic;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// SV3 quantile estimator
    /// <remarks>
    /// Based on the following paper:
    /// Sfakianakis, Michael E., and Dimitris G. Verginis. "A new family of nonparametric quantile estimators."
    /// Communications in Statistics—Simulation and Computation® 37, no. 2 (2008): 337-345.
    /// https://doi.org/10.1080/03610910701790491
    /// </remarks>
    /// </summary>
    public class SfakianakisVerginis3QuantileEstimator : BinomialBasedQuantileEstimator
    {
        public static readonly IQuantileEstimator Instance = new SfakianakisVerginis3QuantileEstimator();

        private SfakianakisVerginis3QuantileEstimator()
        {
        }

        public override string Alias => "SV3";

        protected override double Quantile(IReadOnlyList<double> x, Probability probability, double[] b)
        {
            int n = x.Count;
            double value = 0;
            for (int i = 0; i < n; i++)
                value += b[i + 1] * x[i];
            value += (2 * x[0] - x[1]) * b[0];
            return value;
        }
    }
}