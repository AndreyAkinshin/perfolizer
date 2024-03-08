using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Metrology;

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
        var greaterResult = oneSidedTest.Perform(x, y, alternative, threshold);
        var lesserResult = oneSidedTest.Perform(y, x, alternative, threshold);

        var comparisionResult = ComparisonResult.Equivalent;
        if (greaterResult.PValue < alpha)
            comparisionResult = ComparisonResult.Greater;
        else if (lesserResult.PValue < alpha)
            comparisionResult = ComparisonResult.Lesser;

        return new TostEquivalenceResult<T>(comparisionResult, greaterResult, lesserResult);
    }
}