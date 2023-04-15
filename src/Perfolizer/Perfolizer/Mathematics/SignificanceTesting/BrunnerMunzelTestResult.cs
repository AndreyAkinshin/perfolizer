using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class BrunnerMunzelTestResult : OneSidedTestResult
{
    public double W { get; }
    public double Df { get; }

    public BrunnerMunzelTestResult(double w, double df, double pValue, Threshold threshold) : base(pValue, threshold)
    {
        W = w;
        Df = df;
    }
}