using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.DispersionEstimators;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.OutlierDetection
{
    /// <summary>
    /// Outlier detector based on the median absolute deviation.
    /// Consider all values outside [median - k * MAD, median + k * MAD] as outliers.
    /// </summary>
    public class MadOutlierDetector : FenceOutlierDetector
    {
        private const double DefaultK = 3;

        private static readonly IMedianAbsoluteDeviationEstimator DefaultMedianAbsoluteDeviationEstimator =
            SimpleNormalizedMedianAbsoluteDeviationEstimator.Instance;

        private MadOutlierDetector([NotNull] Sample sample, double k,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            medianAbsoluteDeviationEstimator ??= DefaultMedianAbsoluteDeviationEstimator;
            double median = medianAbsoluteDeviationEstimator.QuantileEstimator.GetMedian(sample);
            double mad = medianAbsoluteDeviationEstimator.Calc(sample);
            LowerFence = median - k * mad;
            UpperFence = median + k * mad;
        }

        [NotNull]
        public static MadOutlierDetector Create([NotNull] Sample sample, double k,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            return new MadOutlierDetector(sample, k, medianAbsoluteDeviationEstimator);
        }

        [NotNull]
        public static MadOutlierDetector Create([NotNull] IReadOnlyList<double> values, double k,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return new MadOutlierDetector(values.ToSample(), k, medianAbsoluteDeviationEstimator);
        }

        [NotNull]
        public static MadOutlierDetector Create([NotNull] Sample sample,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            return new MadOutlierDetector(sample, DefaultK, medianAbsoluteDeviationEstimator);
        }

        [NotNull]
        public static MadOutlierDetector Create([NotNull] IReadOnlyList<double> values,
            [CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return new MadOutlierDetector(values.ToSample(), DefaultK, medianAbsoluteDeviationEstimator);
        }
    }
}