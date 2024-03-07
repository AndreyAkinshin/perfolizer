using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public class TostEquivalenceResult<T>(ComparisonResult comparisonResult, T greaterResult, T lesserResult)
    : EquivalenceResult(comparisonResult)
    where T : SignificanceTwoSampleResult
{
    public T GreaterResult { get; } = greaterResult;
    public T LesserResult { get; } = lesserResult;
}