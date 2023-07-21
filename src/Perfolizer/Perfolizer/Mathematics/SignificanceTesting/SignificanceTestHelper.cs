using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.SignificanceTesting.Base;

namespace Perfolizer.Mathematics.SignificanceTesting;

internal static class SignificanceTestHelper
{
    public static Probability CdfToPValue(double cdf, AlternativeHypothesis alternativeHypothesis) => alternativeHypothesis switch
    {
        AlternativeHypothesis.TwoSides => Min(Min(cdf, 1 - cdf) * 2, 1),
        AlternativeHypothesis.Less => cdf,
        AlternativeHypothesis.Greater => 1 - cdf,
        _ => throw new ArgumentOutOfRangeException(nameof(alternativeHypothesis), alternativeHypothesis, null)
    };
}