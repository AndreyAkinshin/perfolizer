using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Extensions;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public static class QuantileEstimatorExtensions
    {
        [NotNull]
        public static double[] GetQuantilesFromSorted(this IQuantileEstimator estimator, [NotNull] IReadOnlyList<double> data,
            [NotNull] double[] quantiles)
        {
            var results = new double[quantiles.Length];
            for (int i = 0; i < quantiles.Length; i++)
                results[i] = estimator.GetQuantileFromSorted(data, quantiles[i]);

            return results;
        }

        [NotNull]
        public static double[] GetQuantiles([NotNull] this IQuantileEstimator estimator, [NotNull] IReadOnlyList<double> data,
            [NotNull] double[] quantiles)
        {
            return estimator.GetQuantilesFromSorted(data.CopyToArrayAndSort(), quantiles);
        }

        public static double GetQuantile([NotNull] this IQuantileEstimator estimator, [NotNull] IReadOnlyList<double> data, double quantile)
        {
            return estimator.GetQuantileFromSorted(data.CopyToArrayAndSort(), quantile);
        }
    }
}