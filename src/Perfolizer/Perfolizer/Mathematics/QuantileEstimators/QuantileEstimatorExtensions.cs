using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public static class QuantileEstimatorExtensions
    {
        [NotNull]
        public static double[] GetQuantiles(
            [NotNull] this IQuantileEstimator estimator,
            [NotNull] Sample sample,
            [NotNull] IReadOnlyList<Probability> probabilities)
        {
            Assertion.NotNull(nameof(estimator), estimator);
            Assertion.NotNull(nameof(sample), sample);
            Assertion.NotNull(nameof(probabilities), probabilities);

            double[] results = new double[probabilities.Count];
            for (int i = 0; i < probabilities.Count; i++)
                results[i] = estimator.GetQuantile(sample, probabilities[i]);

            return results;
        }

        public static double GetMedian(
            [NotNull] this IQuantileEstimator estimator,
            [NotNull] Sample sample)
        {
            return estimator.GetQuantile(sample, 0.5);
        }

        public static double GetQuantile(
            [NotNull] this IQuantileEstimator estimator,
            [NotNull] IReadOnlyList<double> values,
            Probability probability)
        {
            return estimator.GetQuantile(new Sample(values), probability);
        }

        [NotNull]
        public static double[] GetQuantiles(
            [NotNull] this IQuantileEstimator estimator,
            [NotNull] IReadOnlyList<double> values,
            [NotNull] IReadOnlyList<Probability> probabilities)
        {
            return estimator.GetQuantiles(new Sample(values), probabilities);
        }
    }
}