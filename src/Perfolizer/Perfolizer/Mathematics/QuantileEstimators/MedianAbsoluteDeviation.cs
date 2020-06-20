using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;

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
        /// Equals to 1.4826.
        /// The formula: (StandardDeviation) = 1.4826 * (MedianAbsoluteDeviation).
        /// </summary>
        public const double DefaultConsistencyConstant = 1.4826;
        
        public static double Calc([NotNull] ISortedReadOnlyList<double> values, double consistencyConstant = DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            quantileEstimator ??= SimpleQuantileEstimator.Instance;
            
            double median = quantileEstimator.GetMedian(values);
            var deviations = new double[values.Count];
            for (int i = 0; i < values.Count; i++)
                deviations[i] = Math.Abs(values[i] - median);
            return consistencyConstant * quantileEstimator.GetMedian(deviations);
        }

        public static double Calc([NotNull] IReadOnlyList<double> values, double consistencyConstant = DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null) =>
            Calc(values.ToSorted(), consistencyConstant, quantileEstimator);
    }
}