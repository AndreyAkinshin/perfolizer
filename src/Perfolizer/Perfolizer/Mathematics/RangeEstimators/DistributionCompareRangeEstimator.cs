using System;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;

namespace Perfolizer.Mathematics.RangeEstimators
{
    public class DistributionCompareRangeEstimator
    {
        private readonly DistributionCompareFunction distributionCompareFunction;

        public DistributionCompareRangeEstimator(DistributionCompareFunction distributionCompareFunction)
        {
            this.distributionCompareFunction = distributionCompareFunction;
        }

        public Range GetRange([NotNull] Sample a, [NotNull] Sample b, double margin)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);
            Assertion.InRangeInclusive(nameof(margin), margin, 0, 0.5);
            
            int minQuantile = Math.Min(50, (int) Math.Round(margin * 100));
            int maxQuantile = 100 - minQuantile;
            double[] probabilities = new double[maxQuantile - minQuantile + 1];
            for (int i = 0; i < probabilities.Length; i++)
                probabilities[i] = (minQuantile + i) / 100.0;
            double[] quantileValues = distributionCompareFunction.GetValues(a, b, probabilities);
            return Range.Of(quantileValues.Min(), quantileValues.Max());
        }

        public Range GetRange([NotNull] Sample a, [NotNull] Sample b)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);
            
            int n = Math.Min(a.Count, b.Count);
            double margin = Math.Min(0.5, 1 - 0.001.Pow(1.0 / n));
            return GetRange(a, b, margin);
        }
    }
}