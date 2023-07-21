using Perfolizer.Common;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public interface ISignificanceTwoSampleTest<out T> where T : SignificanceTwoSampleResult
{
    T Run(Sample x, Sample y, AlternativeHypothesis alternativeHypothesis = AlternativeHypothesis.Greater, Threshold? threshold = null);
}