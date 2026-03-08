using Perfolizer.Metrology;
using Pragmastat;
using Threshold = Perfolizer.Metrology.Threshold;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

[Obsolete("Use Pragmastat.Toolkit.Compare2 instead.")]
public interface ISignificanceTwoSampleTest
{
    Probability GetPValue(Sample x, Sample y, AlternativeHypothesis alternative, Threshold threshold);
}

[Obsolete("Use Pragmastat.Toolkit.Compare2 instead.")]
public interface ISignificanceTwoSampleTest<out T> : ISignificanceTwoSampleTest where T : SignificanceTwoSampleResult
{
    T Perform(Sample x, Sample y, AlternativeHypothesis alternative, Threshold threshold);
}