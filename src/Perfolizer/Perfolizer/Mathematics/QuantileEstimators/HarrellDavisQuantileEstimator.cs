using Perfolizer.Common;
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

        public double GetQuantile(Sample sample, double probability)
        {
            Assertion.NotNull(nameof(sample), sample);
            Assertion.InRangeInclusive(nameof(probability), probability, 0, 1);

            int n = sample.Count;
            double a = (n + 1) * probability, b = (n + 1) * (1 - probability);
            var distribution = new BetaDistribution(a, b);

            double result = 0;
            double betaCdfRight = 0;
            double currentProbability = 0;
            for (int j = 0; j < sample.Count; j++)
            {
                double betaCdfLeft = betaCdfRight;
                currentProbability += sample.SortedWeights[j] / sample.TotalWeight;
                betaCdfRight = distribution.Cdf(currentProbability);
                result += (betaCdfRight - betaCdfLeft) * sample.SortedValues[j];
            }

            return result;
        }

        public bool SupportsWeightedSamples => true;
    }
}