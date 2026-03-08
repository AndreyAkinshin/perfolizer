using Pragmastat;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

[Obsolete("Use Pragmastat.Toolkit.Compare1 instead.")]
public interface ISignificanceOneSampleTest<out T> where T : SignificanceOneSampleResult
{
    T Run(Sample x, double y, AlternativeHypothesis alternativeHypothesis);
}