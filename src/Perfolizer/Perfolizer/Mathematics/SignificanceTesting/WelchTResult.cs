using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Metrology;
using Pragmastat;
using Threshold = Perfolizer.Metrology.Threshold;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class WelchTResult : SignificanceTwoSampleResult
{
    public double T { get; }
    public double Df { get; }

    public WelchTResult(Sample x, Sample y, Threshold threshold, AlternativeHypothesis alternativeHypothesis, Probability pValue, double t,
        double df) : base(x, y, threshold, alternativeHypothesis, pValue)
    {
        T = t;
        Df = df;
    }
}