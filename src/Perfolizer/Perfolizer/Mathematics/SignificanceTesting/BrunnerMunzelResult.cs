using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Metrology;
using Pragmastat;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class BrunnerMunzelResult : SignificanceTwoSampleResult
{
    public double W { get; }
    public double Df { get; }

    public BrunnerMunzelResult(Sample x, Sample y, Threshold threshold, AlternativeHypothesis alternativeHypothesis, Probability pValue,
        double w, double df) : base(x, y, threshold, alternativeHypothesis, pValue)
    {
        W = w;
        Df = df;
    }
}