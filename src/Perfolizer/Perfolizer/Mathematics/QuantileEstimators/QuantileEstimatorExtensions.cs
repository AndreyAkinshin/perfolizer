using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public static class QuantileEstimatorExtensions
    {
        public static double[] GetQuantiles(
            this IQuantileEstimator estimator,
            Sample sample,
            IReadOnlyList<Probability> probabilities)
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
            this IQuantileEstimator estimator,
            Sample sample)
        {
            return estimator.GetQuantile(sample, 0.5);
        }

        public static double GetQuantile(
            this IQuantileEstimator estimator,
            IReadOnlyList<double> values,
            Probability probability)
        {
            return estimator.GetQuantile(new Sample(values), probability);
        }

        public static double[] GetQuantiles(
            this IQuantileEstimator estimator,
            IReadOnlyList<double> values,
            IReadOnlyList<Probability> probabilities)
        {
            return estimator.GetQuantiles(new Sample(values), probabilities);
        }
    }
}