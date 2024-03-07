using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public abstract class SignificanceTwoSampleTestBase<T> : ISignificanceTwoSampleTest<T>
    where T : SignificanceTwoSampleResult
{
    public abstract T Perform(Sample x, Sample y, AlternativeHypothesis alternative, Threshold threshold);

    public Probability GetPValue(Sample x, Sample y, AlternativeHypothesis alternative, Threshold threshold)
    {
        var result = Perform(x, y, alternative, threshold);
        return result.PValue;
    }
}