using Perfolizer.Common;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public interface ISignificanceOneSampleTest<out T> where T : SignificanceOneSampleResult
{
    T Run(Sample x, double y, AlternativeHypothesis alternativeHypothesis);
}