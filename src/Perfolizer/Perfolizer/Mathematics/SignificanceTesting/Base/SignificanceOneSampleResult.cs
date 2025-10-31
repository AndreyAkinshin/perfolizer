using Perfolizer.Common;
using Pragmastat;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public class SignificanceOneSampleResult
{
    public Sample X { get; }
    public double Y { get; }
    public AlternativeHypothesis AlternativeHypothesis { get; }
    public Probability PValue { get; }

    public SignificanceOneSampleResult(Sample x, double y, AlternativeHypothesis alternativeHypothesis, Probability pValue)
    {
        X = x;
        Y = y;
        AlternativeHypothesis = alternativeHypothesis;
        PValue = pValue;
    }

    public string PValueString => PValue.ToString("N4", DefaultCultureInfo.Instance);

    public override string ToString() => $"{nameof(PValue)}: {PValueString}";
}