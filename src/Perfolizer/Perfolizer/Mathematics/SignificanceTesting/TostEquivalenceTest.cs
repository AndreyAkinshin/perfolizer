using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

/// <summary>
/// TOST (Two One-Sided Tests) equivalence test
/// </summary>
public class TostEquivalenceTest<T>(ISignificanceTwoSampleTest<T> oneSidedTest)
    : ITostEquivalenceTest<T> where T : SignificanceTwoSampleResult
{
    public bool AreEquivalent(Sample x, Sample y, Threshold threshold, SignificanceLevel alpha)
    {
        return Perform(x, y, threshold, alpha).ComparisonResult == ComparisonResult.Equivalent;
    }

    public TostEquivalenceResult<T> Perform(Sample x, Sample y, Threshold threshold, SignificanceLevel alpha)
    {
        const AlternativeHypothesis alternative = AlternativeHypothesis.Greater;
        var zero = AbsoluteThreshold.Zero;
        var greaterResult = oneSidedTest.Perform(threshold.Apply(x), y, alternative, zero);
        var lesserResult = oneSidedTest.Perform(threshold.Apply(y), x, alternative, zero);
        var comparisionResult = greaterResult.PValue < alpha && lesserResult.PValue < alpha
            ? ComparisonResult.Equivalent
            : HodgesLehmannEstimator.Instance.Shift(x, y) > 0
                ? ComparisonResult.Greater
                : ComparisonResult.Lesser;

        return new TostEquivalenceResult<T>(comparisionResult, greaterResult, lesserResult);
    }
}