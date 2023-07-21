using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.SignificanceTesting.Base;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class StudentTOneSampleResult : SignificanceOneSampleResult
{
    public double T { get; }
    public double Df { get; }

    public StudentTOneSampleResult(Sample x, double y, AlternativeHypothesis alternativeHypothesis, Probability pValue, double t, double df)
        : base(x, y, alternativeHypothesis, pValue)
    {
        T = t;
        Df = df;
    }
}