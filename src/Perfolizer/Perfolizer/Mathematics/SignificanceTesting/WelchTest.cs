using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class WelchTest : ISignificanceTwoSampleTest<WelchTResult>
{
    public static readonly WelchTest Instance = new();

    public WelchTResult Run(Sample x, Sample y, AlternativeHypothesis alternativeHypothesis = AlternativeHypothesis.Greater,
        Threshold? threshold = null)
    {
        Assertion.SizeLargerThan(nameof(x), x, 1);
        Assertion.SizeLargerThan(nameof(y), y, 1);
        threshold ??= AbsoluteThreshold.Zero;
        y = threshold.Apply(y);

        int n1 = x.Count, n2 = y.Count;

        Moments xm = Moments.Create(x), ym = Moments.Create(y);
        double v1 = xm.Variance, v2 = ym.Variance, m1 = xm.Mean, m2 = ym.Mean;

        double se = Sqrt(v1 / n1 + v2 / n2);
        double t = (m1 - m2) / se;
        double df = (v1 / n1 + v2 / n2).Sqr() /
                    ((v1 / n1).Sqr() / (n1 - 1) + (v2 / n2).Sqr() / (n2 - 1));
        double cdf = new StudentDistribution(df).Cdf(t);
        double pValue = SignificanceTestHelper.CdfToPValue(cdf, alternativeHypothesis);

        return new WelchTResult(x, y, threshold, alternativeHypothesis, pValue, t, df);
    }
}