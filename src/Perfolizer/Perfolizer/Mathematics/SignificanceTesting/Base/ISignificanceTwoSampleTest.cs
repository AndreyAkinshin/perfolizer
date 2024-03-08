using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public interface ISignificanceTwoSampleTest
{
    Probability GetPValue(Sample x, Sample y, AlternativeHypothesis alternative, Threshold threshold);
}

public interface ISignificanceTwoSampleTest<out T> : ISignificanceTwoSampleTest where T : SignificanceTwoSampleResult
{
    T Perform(Sample x, Sample y, AlternativeHypothesis alternative, Threshold threshold);
}