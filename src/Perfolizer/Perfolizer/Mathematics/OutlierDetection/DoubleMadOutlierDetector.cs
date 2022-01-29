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

        private DoubleMadOutlierDetector(Sample sample, double k,
            IMedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            medianAbsoluteDeviationEstimator ??= DefaultMedianAbsoluteDeviationEstimator;
            double median = medianAbsoluteDeviationEstimator.QuantileEstimator.GetMedian(sample);
            double lowerMad = medianAbsoluteDeviationEstimator.CalcLower(sample);
            double upperMad = medianAbsoluteDeviationEstimator.CalcUpper(sample);
            LowerFence = median - k * lowerMad;
            UpperFence = median + k * upperMad;
        }

        public static DoubleMadOutlierDetector Create(Sample sample, double k,
            IMedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            return new DoubleMadOutlierDetector(sample, k, medianAbsoluteDeviationEstimator);
        }

        public static DoubleMadOutlierDetector Create(IReadOnlyList<double> values, double k,
            IMedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return new DoubleMadOutlierDetector(values.ToSample(), k, medianAbsoluteDeviationEstimator);
        }

        public static DoubleMadOutlierDetector Create(Sample sample,
            IMedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            return new DoubleMadOutlierDetector(sample, DefaultK, medianAbsoluteDeviationEstimator);
        }

        public static DoubleMadOutlierDetector Create(IReadOnlyList<double> values,
            IMedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return new DoubleMadOutlierDetector(values.ToSample(), DefaultK, medianAbsoluteDeviationEstimator);
        }
    }
}