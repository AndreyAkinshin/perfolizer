using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Metrology;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class WelchTest : SignificanceTwoSampleTestBase<WelchTResult>
{
    public static readonly WelchTest Instance = new();

    public override WelchTResult Perform(
        Sample x,
        Sample y,
        AlternativeHypothesis alternative,
        Threshold threshold)
    {
        Assertion.SizeLargerThan(nameof(x), x, 1);
        Assertion.SizeLargerThan(nameof(y), y, 1);
        y = threshold.ApplyMax(y);

        int n1 = x.Size, n2 = y.Size;

        Moments xm = Moments.Create(x), ym = Moments.Create(y);
        double v1 = xm.Variance, v2 = ym.Variance, m1 = xm.Mean, m2 = ym.Mean;

        double se = Sqrt(v1 / n1 + v2 / n2);
        double t = (m1 - m2) / se;
        double df = (v1 / n1 + v2 / n2).Sqr() /
                    ((v1 / n1).Sqr() / (n1 - 1) + (v2 / n2).Sqr() / (n2 - 1));
        double cdf = new StudentDistribution(df).Cdf(t);
        double pValue = SignificanceTestHelper.CdfToPValue(cdf, alternative);

        return new WelchTResult(x, y, threshold, alternative, pValue, t, df);
    }
}