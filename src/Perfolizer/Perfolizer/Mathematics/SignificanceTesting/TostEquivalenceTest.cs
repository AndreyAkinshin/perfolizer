using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

/// <summary>
/// TOST (Two One-Sided Tests) equivalence test
/// </summary>
public class TostEquivalenceTest(ISignificanceTwoSampleTest oneSidedTest) : IEquivalenceTest
{
    private double GetPValue(Sample x, Sample y) =>
        oneSidedTest.GetPValue(x, y, AlternativeHypothesis.Greater, AbsoluteThreshold.Zero);

    public bool AreEquivalent(Sample x, Sample y, Threshold threshold, SignificanceLevel alpha)
    {
        double pValue1 = GetPValue(threshold.Apply(x), y);
        double pValue2 = GetPValue(threshold.Apply(y), x);
        return pValue1 < alpha && pValue2 < alpha;
    }
}