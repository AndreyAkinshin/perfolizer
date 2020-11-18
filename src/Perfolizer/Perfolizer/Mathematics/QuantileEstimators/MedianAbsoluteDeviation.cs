using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// The median absolute deviation (MAD).
    /// MAD = median(abs(x[i] - median(x)))
    /// </summary>
    public static class MedianAbsoluteDeviation
    {
        /// <summary>
        /// Ratio between the standard deviation and the median absolute deviation for the normal distribution.
        /// It equals ≈1.482602218505602.
        /// The formula: (StandardDeviation) = 1.482602218505602 * (MedianAbsoluteDeviation).
        /// </summary>
        public const double DefaultConsistencyConstant = 1.482602218505602;

        public static double CalcMad(
            [NotNull] Sample sample,
            double consistencyConstant = DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);
            quantileEstimator ??= SimpleQuantileEstimator.Instance;

            double median = quantileEstimator.GetMedian(sample);
            double[] deviations = new double[sample.Count];
            for (int i = 0; i < sample.Count; i++)
                deviations[i] = Math.Abs(sample.Values[i] - median);
            return consistencyConstant * quantileEstimator.GetMedian(new Sample(deviations));
        }

        public static double CalcLowerMad(
            [NotNull] Sample sample,
            double consistencyConstant = DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);
            quantileEstimator ??= SimpleQuantileEstimator.Instance;

            double median = quantileEstimator.GetMedian(sample);
            var deviations = new List<double>(sample.Count);
            for (int i = 0; i < sample.Count; i++)
                if (sample.Values[i] <= median)
                    deviations.Add(Math.Abs(sample.Values[i] - median));
            return consistencyConstant * quantileEstimator.GetMedian(new Sample(deviations));
        }

        public static double CalcUpperMad(
            [NotNull] Sample sample,
            double consistencyConstant = DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);
            quantileEstimator ??= SimpleQuantileEstimator.Instance;

            double median = quantileEstimator.GetMedian(sample);
            var deviations = new List<double>(sample.Count);
            for (int i = 0; i < sample.Count; i++)
                if (sample.Values[i] >= median)
                    deviations.Add(Math.Abs(sample.Values[i] - median));
            return consistencyConstant * quantileEstimator.GetMedian(new Sample(deviations));
        }
    }
}