using Perfolizer.Common;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public interface ISignificanceTwoSampleTest<out T> where T : SignificanceTwoSampleResult
{
    T Perform(Sample x, Sample y, AlternativeHypothesis alternative, Threshold threshold);
}