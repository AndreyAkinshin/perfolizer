using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public class EquivalenceResult(ComparisonResult comparisonResult)
{
    [PublicAPI]
    public ComparisonResult ComparisonResult { get; } = comparisonResult;
}