using System;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.QuantileEstimators;

/// <summary>
/// Quantile absolute deviation (QAD).
/// See https://aakinshin.net/posts/qad/
/// </summary>
public static class QuantileAbsoluteDeviation
{
    /// <summary>
    /// Ratio between the standard deviation and the median absolute deviation for the normal distribution.
    /// It equals ≈1.482602218505602.
    /// The formula: (StandardDeviation) = 1.482602218505602 * (MedianAbsoluteDeviation).
    /// </summary>
    public const double DefaultConsistencyConstant = 1.482602218505602;

    public static double CalcQad(Sample sample,
        double p,
        double q,
        double consistencyConstant = DefaultConsistencyConstant,
        IQuantileEstimator? quantileEstimator = null)
    {
        Assertion.NotNull(nameof(sample), sample);
        quantileEstimator ??= SimpleQuantileEstimator.Instance;

        double quantile = quantileEstimator.Quantile(sample, p);
        double[] deviations = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
            deviations[i] = Math.Abs(sample.Values[i] - quantile);
        return consistencyConstant * quantileEstimator.Quantile(new Sample(deviations), q);
    }
}