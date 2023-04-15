using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class TostResult<T> : EquivalenceTestResult where T : OneSidedTestResult
{
    public T? SlowerTestResult { get; }

    public T? FasterTestResult { get; }

    public TostResult(Threshold threshold, EquivalenceTestConclusion conclusion, T? slowerTestResult, T? fasterTestResult)
        : base(threshold, conclusion)
    {
        SlowerTestResult = slowerTestResult;
        FasterTestResult = fasterTestResult;
    }

    public string ToString(bool details) => details
        ? ConclusionString() + ": " + (SlowerTestResult?.PValueStr ?? "?") + "|" + (FasterTestResult?.PValueStr ?? "?")
        : ConclusionString();

    private string ConclusionString() => Conclusion == EquivalenceTestConclusion.Unknown ? "?" : Conclusion.ToString();
}