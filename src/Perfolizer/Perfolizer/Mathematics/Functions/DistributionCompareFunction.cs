using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public abstract class DistributionCompareFunction
    {
        [NotNull] private readonly IQuantileEstimator quantileEstimator;

        protected DistributionCompareFunction([CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            this.quantileEstimator = quantileEstimator ?? HarrellDavisQuantileEstimator.Instance;
        }

        public double GetValue([NotNull] Sample a, [NotNull] Sample b, double probability)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);
            Assertion.InRangeInclusive(nameof(probability), probability, 0, 1);

            double quantileA = quantileEstimator.GetQuantile(a, probability);
            double quantileB = quantileEstimator.GetQuantile(b, probability);
            return CalculateValue(quantileA, quantileB);
        }

        [NotNull]
        public double[] GetValues([NotNull] Sample a, [NotNull] Sample b, [NotNull] double[] probabilities)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);
            Assertion.InRangeInclusive(nameof(probabilities), probabilities, 0, 1);

            double[] quantilesA = quantileEstimator.GetQuantiles(a, probabilities);
            double[] quantilesB = quantileEstimator.GetQuantiles(b, probabilities);
            double[] values = new double[probabilities.Length];
            for (int i = 0; i < values.Length; i++)
                values[i] = CalculateValue(quantilesA[i], quantilesB[i]);
            return values;
        }

        protected abstract double CalculateValue(double quantileA, double quantileB);
    }
}