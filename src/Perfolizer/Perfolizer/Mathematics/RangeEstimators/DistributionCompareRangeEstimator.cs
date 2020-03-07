using System;
using System.Linq;
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

        public Range GetRange(double[] a, double[] b, double margin)
        {
            if (margin < 0 || margin > 0.5)
                throw new ArgumentOutOfRangeException(nameof(margin), $"{nameof(margin)} should be inside [0;0.5]");
            int minQuantile = Math.Min(50, (int) Math.Round(margin * 100));
            int maxQuantile = 100 - minQuantile;
            var quantilePositions = new double[maxQuantile - minQuantile + 1];
            for (int i = 0; i < quantilePositions.Length; i++)
                quantilePositions[i] = (minQuantile + i) / 100.0;
            var quantileValues = distributionCompareFunction.Values(a, b, quantilePositions);
            return Range.Of(quantileValues.Min(), quantileValues.Max());
        }

        public Range GetRange(double[] a, double[] b)
        {
            int n = Math.Min(a.Length, b.Length);
            double margin = Math.Min(0.5, 1 - 0.001.Pow(1.0 / n));
            return GetRange(a, b, margin);
        }
    }
}