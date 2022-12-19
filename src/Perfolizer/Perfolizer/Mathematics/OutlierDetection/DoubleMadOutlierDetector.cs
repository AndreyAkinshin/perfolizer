using System.Collections.Generic;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.ScaleEstimators;

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

        private static readonly MedianAbsoluteDeviationEstimator DefaultMedianAbsoluteDeviationEstimator =
            MedianAbsoluteDeviationEstimator.Simple;

        private DoubleMadOutlierDetector(Sample sample, double k,
            MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            medianAbsoluteDeviationEstimator ??= DefaultMedianAbsoluteDeviationEstimator;
            double median = medianAbsoluteDeviationEstimator.QuantileEstimator.Median(sample);
            double lowerMad = medianAbsoluteDeviationEstimator.LowerMad(sample);
            double upperMad = medianAbsoluteDeviationEstimator.UpperMad(sample);
            LowerFence = median - k * lowerMad;
            UpperFence = median + k * upperMad;
        }

        public static DoubleMadOutlierDetector Create(Sample sample, double k,
            MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            return new DoubleMadOutlierDetector(sample, k, medianAbsoluteDeviationEstimator);
        }

        public static DoubleMadOutlierDetector Create(IReadOnlyList<double> values, double k,
            MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return new DoubleMadOutlierDetector(values.ToSample(), k, medianAbsoluteDeviationEstimator);
        }

        public static DoubleMadOutlierDetector Create(Sample sample,
            MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            return new DoubleMadOutlierDetector(sample, DefaultK, medianAbsoluteDeviationEstimator);
        }

        public static DoubleMadOutlierDetector Create(IReadOnlyList<double> values,
            MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return new DoubleMadOutlierDetector(values.ToSample(), DefaultK, medianAbsoluteDeviationEstimator);
        }
    }
}