using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.GenericEstimators;

/// <summary>
/// Based on:
/// Hodges, J. L., and E. L. Lehmann. 1963. Estimates of location based on rank tests.
/// The Annals of Mathematical Statistics 34 (2):598–611.  
/// DOI: 10.1214/aoms/1177704172
///
/// The weighted version is based on https://aakinshin.net/posts/whl/
/// </summary>
public class HodgesLehmannEstimator(IQuantileEstimator quantileEstimator)
    : IShiftEstimator, IMedianEstimator, IRatioEstimator
{
    public static readonly HodgesLehmannEstimator Instance = new(SimpleQuantileEstimator.Instance);

    /// <summary>
    /// Pseudo-median: the median of the Walsh (pairwise) averages
    /// </summary>
    public double Median(Sample x) =>
        PairwiseEstimatorHelper.Estimate(x, (xi, xj) => (xi + xj) / 2, quantileEstimator, Probability.Median, true);

    public double Shift(Sample x, Sample y) => Estimate(x, y, (xi, yj) => xi - yj);

    public double Ratio(Sample x, Sample y) => Estimate(x, y, (xi, yj) => xi / yj);

    private double Estimate(Sample x, Sample y, Func<double, double, double> func) =>
        PairwiseEstimatorHelper.Estimate(x, y, func, quantileEstimator, Probability.Median);
}