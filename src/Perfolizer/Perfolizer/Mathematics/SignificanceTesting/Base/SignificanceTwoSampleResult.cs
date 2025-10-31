using Perfolizer.Common;
using Perfolizer.Metrology;
using Pragmastat;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public class SignificanceTwoSampleResult
{
    public Sample X { get; }
    public Sample Y { get; }
    public Threshold Threshold { get; }
    public AlternativeHypothesis AlternativeHypothesis { get; }
    public Probability PValue { get; }

    public SignificanceTwoSampleResult(Sample x, Sample y, Threshold threshold, AlternativeHypothesis alternativeHypothesis,
        Probability pValue)
    {
        X = x;
        Y = y;
        Threshold = threshold;
        AlternativeHypothesis = alternativeHypothesis;
        PValue = pValue;
    }

    public string PValueString => PValue.ToString("N4", DefaultCultureInfo.Instance);

    public override string ToString() => $"{nameof(PValue)}: {PValueString}";
}