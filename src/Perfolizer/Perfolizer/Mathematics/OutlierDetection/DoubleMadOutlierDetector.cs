using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.OutlierDetection
{
    /// <summary>
    /// Outlier detector based on the double median absolute deviation.
    /// Consider all values outside [median - consistencyConstant * k * LowerMAD, median + consistencyConstant * k * UpperMAD] as outliers.
    /// <remarks>
    /// By default, it uses the Harrell Davis quantile estimator, consistencyConstant = 1.4826, and k = 3.
    /// See also: https://eurekastatistics.com/using-the-median-absolute-deviation-to-find-outliers/
    /// </remarks>
    /// </summary>
    public class DoubleMadOutlierDetector : FenceOutlierDetector
    {
        private const double DefaultK = 3;

        private DoubleMadOutlierDetector([NotNull] ISortedReadOnlyList<double> values, double k, double consistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator)
        {
            if (values.Count == 0)
            {
                HandleEmptySample();
                return;
            }

            quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
            double median = quantileEstimator.GetMedian(values);
            double lowerMad = MedianAbsoluteDeviation.CalcLowerMad(values, consistencyConstant, quantileEstimator);
            double upperMad = MedianAbsoluteDeviation.CalcUpperMad(values, consistencyConstant, quantileEstimator);
            LowerFence = median - k * lowerMad;
            UpperFence = median + k * upperMad;
        }

        [NotNull]
        public static DoubleMadOutlierDetector Create([NotNull] ISortedReadOnlyList<double> values, double k = DefaultK,
            double consistencyConstant = MedianAbsoluteDeviation.DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return new DoubleMadOutlierDetector(values, k, consistencyConstant, quantileEstimator);
        }

        [NotNull]
        public static DoubleMadOutlierDetector Create([NotNull] IReadOnlyList<double> values, double k = DefaultK,
            double consistencyConstant = MedianAbsoluteDeviation.DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return new DoubleMadOutlierDetector(values.ToSorted(), k, consistencyConstant, quantileEstimator);
        }
    }
}