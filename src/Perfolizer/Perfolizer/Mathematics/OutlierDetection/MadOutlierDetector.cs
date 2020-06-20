using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.OutlierDetection
{
    /// <summary>
    /// Outlier detector based on the median absolute deviation.
    /// Consider all values outside [median - consistencyConstant * k * MAD, median + consistencyConstant * k * MAD] as outliers.
    /// <remarks>
    /// By default, it uses the Harrell Davis quantile estimator, consistencyConstant = 1.4826, and k = 3.
    /// </remarks>
    /// </summary>
    public class MadOutlierDetector : FenceOutlierDetector
    {
        private const double DefaultK = 3;

        private MadOutlierDetector([NotNull] ISortedReadOnlyList<double> values, double k, double consistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator)
        {
            quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
            double median = quantileEstimator.GetMedian(values);
            double mad = MedianAbsoluteDeviation.CalcMad(values, consistencyConstant, quantileEstimator);
            LowerFence = median - k * mad;
            UpperFence = median + k * mad;
        }

        [NotNull]
        public static MadOutlierDetector Create([NotNull] ISortedReadOnlyList<double> values, double k = DefaultK,
            double consistencyConstant = MedianAbsoluteDeviation.DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
            => new MadOutlierDetector(values, k, consistencyConstant, quantileEstimator);

        [NotNull]
        public static MadOutlierDetector Create([NotNull] IReadOnlyList<double> values, double k = DefaultK,
            double consistencyConstant = MedianAbsoluteDeviation.DefaultConsistencyConstant,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
            => new MadOutlierDetector(values.ToSorted(), k, consistencyConstant, quantileEstimator);
    }
}