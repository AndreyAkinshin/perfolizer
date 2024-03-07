using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

public static class SignificanceTestHelper
{
    public static bool AreEquivalent(
        ISignificanceTwoSampleTest test,
        Sample x,
        Sample y,
        Threshold threshold,
        SignificanceLevel alpha)
    {
        var deltas = DeltasEstimator.HodgesLehmannShamos.Deltas(x, y);
        var pValue = test.GetPValue(x, y, AlternativeHypothesis.TwoSides, threshold);
        return pValue < alpha;
    }

    internal static Probability CdfToPValue(double cdf, AlternativeHypothesis alternative) => alternative switch
    {
        AlternativeHypothesis.TwoSides => Min(Min(cdf, 1 - cdf) * 2, 1),
        AlternativeHypothesis.Less => cdf,
        AlternativeHypothesis.Greater => 1 - cdf,
        _ => throw new ArgumentOutOfRangeException(nameof(alternative), alternative, null)
    };
}