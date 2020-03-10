using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Extensions;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public static class QuantileEstimatorExtensions
    {
        [NotNull]
        public static double[] GetQuantilesFromSorted(this IQuantileEstimator estimator, [NotNull] IReadOnlyList<double> data,
            [NotNull] double[] probabilities)
        {
            var results = new double[probabilities.Length];
            for (int i = 0; i < probabilities.Length; i++)
                results[i] = estimator.GetQuantileFromSorted(data, probabilities[i]);

            return results;
        }

        [NotNull]
        public static double[] GetQuantiles([NotNull] this IQuantileEstimator estimator, [NotNull] IReadOnlyList<double> data,
            [NotNull] double[] probabilities)
        {
            return estimator.GetQuantilesFromSorted(data.CopyToArrayAndSort(), probabilities);
        }

        public static double GetQuantile([NotNull] this IQuantileEstimator estimator, [NotNull] IReadOnlyList<double> data,
            double probability)
        {
            return estimator.GetQuantileFromSorted(data.CopyToArrayAndSort(), probability);
        }
    }
}