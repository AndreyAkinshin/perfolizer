using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.ScaleEstimators;
using Pragmastat;

namespace Perfolizer.Mathematics.OutlierDetection;

/// <summary>
/// Outlier detector based on the median absolute deviation.
/// Consider all values outside [median - k * MAD, median + k * MAD] as outliers.
/// </summary>
public class MadOutlierDetector : FenceOutlierDetector
{
    private const double DefaultK = 3;

    private static readonly MedianAbsoluteDeviationEstimator DefaultMedianAbsoluteDeviationEstimator =
        MedianAbsoluteDeviationEstimator.Simple;

    private MadOutlierDetector(Sample sample, double k,
        MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
    {
        Assertion.NotNull(nameof(sample), sample);

        medianAbsoluteDeviationEstimator ??= DefaultMedianAbsoluteDeviationEstimator;
        double median = medianAbsoluteDeviationEstimator.QuantileEstimator.Median(sample);
        double mad = medianAbsoluteDeviationEstimator.Mad(sample);
        LowerFence = median - k * mad;
        UpperFence = median + k * mad;
    }

    public static MadOutlierDetector Create(Sample sample, double k,
        MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
    {
        Assertion.NotNull(nameof(sample), sample);

        return new MadOutlierDetector(sample, k, medianAbsoluteDeviationEstimator);
    }

    public static MadOutlierDetector Create(IReadOnlyList<double> values, double k,
        MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
    {
        Assertion.NotNull(nameof(values), values);

        return new MadOutlierDetector(values.ToSample(), k, medianAbsoluteDeviationEstimator);
    }

    public static MadOutlierDetector Create(Sample sample,
        MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
    {
        Assertion.NotNull(nameof(sample), sample);

        return new MadOutlierDetector(sample, DefaultK, medianAbsoluteDeviationEstimator);
    }

    public static MadOutlierDetector Create(IReadOnlyList<double> values,
        MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null)
    {
        Assertion.NotNull(nameof(values), values);

        return new MadOutlierDetector(values.ToSample(), DefaultK, medianAbsoluteDeviationEstimator);
    }
}