using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.DispersionEstimators;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.OutlierDetection
{
    /// <summary>
    /// Outlier detector based on the double median absolute deviation.
    /// Consider all values outside [median - k * LowerMAD, median + k * UpperMAD] as outliers.
    /// <remarks>
    /// See also: https://eurekastatistics.com/using-the-median-absolute-deviation-to-find-outliers/
    /// </remarks>
    /// </summary>
    public class DoubleMadOutlierDetector : FenceOutlierDetector
    {
        private const double DefaultK = 3;

        private static readonly IMedianAbsoluteDeviationEstimator DefaultMedianAbsoluteDeviationEstimator =
            SimpleNormalizedMedianAbsoluteDeviationEstimator.Instance;

        private DoubleMadOutlierDetector([NotNull] Sample sample, double k,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            medianAbsoluteDeviationEstimator ??= DefaultMedianAbsoluteDeviationEstimator;
            double median = medianAbsoluteDeviationEstimator.QuantileEstimator.GetMedian(sample);
            double lowerMad = medianAbsoluteDeviationEstimator.CalcLower(sample);
            double upperMad = medianAbsoluteDeviationEstimator.CalcUpper(sample);
            LowerFence = median - k * lowerMad;
            UpperFence = median + k * upperMad;
        }

        [NotNull]
        public static DoubleMadOutlierDetector Create([NotNull] Sample sample, double k,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            return new DoubleMadOutlierDetector(sample, k, medianAbsoluteDeviationEstimator);
        }

        [NotNull]
        public static DoubleMadOutlierDetector Create([NotNull] IReadOnlyList<double> values, double k,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return new DoubleMadOutlierDetector(values.ToSample(), k, medianAbsoluteDeviationEstimator);
        }

        [NotNull]
        public static DoubleMadOutlierDetector Create([NotNull] Sample sample,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            return new DoubleMadOutlierDetector(sample, DefaultK, medianAbsoluteDeviationEstimator);
        }

        [NotNull]
        public static DoubleMadOutlierDetector Create([NotNull] IReadOnlyList<double> values,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return new DoubleMadOutlierDetector(values.ToSample(), DefaultK, medianAbsoluteDeviationEstimator);
        }
    }
}