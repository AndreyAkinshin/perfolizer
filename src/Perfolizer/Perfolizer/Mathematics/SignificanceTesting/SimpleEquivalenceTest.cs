using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Metrology;

namespace Perfolizer.Mathematics.SignificanceTesting;

// TODO: replace the dummy implementation with a reliable one
public class SimpleEquivalenceTest(ISignificanceTwoSampleTest oneSidedTest) : IEquivalenceTest
{
    public ComparisonResult Perform(Sample x, Sample y, Threshold threshold, SignificanceLevel alpha)
    {
        if (x.Size <= 1 && y.Size <= 1)
            return ComparisonResult.Unknown;

        var deltas = DeltasEstimator.HodgesLehmannShamos.Deltas(x, y);
        double thresholdShift = Max(threshold.EffectiveShift(x), threshold.EffectiveShift(y));

        // Practical significance
        if (thresholdShift > 0 && deltas.Shift.Abs() > thresholdShift * 10)
            return deltas.Shift > 0 ? ComparisonResult.Greater : ComparisonResult.Lesser;

        // Statistical significance (TOST)
        const AlternativeHypothesis alternative = AlternativeHypothesis.Greater;
        var greaterPValue = oneSidedTest.GetPValue(x, y, alternative, threshold);
        var lesserPValue = oneSidedTest.GetPValue(y, x, alternative, threshold);

        var comparisionResult = ComparisonResult.Indistinguishable;
        if (greaterPValue < alpha)
            comparisionResult = ComparisonResult.Greater;
        else if (lesserPValue < alpha)
            comparisionResult = ComparisonResult.Lesser;

        return comparisionResult;
    }
}