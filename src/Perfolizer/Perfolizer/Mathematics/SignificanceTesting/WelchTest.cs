using System;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class WelchTest : IOneSidedTest<WelchResult>
{
    public static readonly WelchTest Instance = new WelchTest();

    /// <summary>
    /// Checks that (x-y) > threshold
    /// </summary>
    /// <remarks>Should be consistent with t.test(x, y, mu=threshold, alternative="greater") from R</remarks>
    public WelchResult IsGreater(double[] x, double[] y, Threshold? threshold = null)
    {
        int n1 = x.Length, n2 = y.Length;
        if (n1 < 2)
            throw new ArgumentException("x should contains at least 2 elements", nameof(x));
        if (n2 < 2)
            throw new ArgumentException("y should contains at least 2 elements", nameof(y));

        Moments xm = Moments.Create(x), ym = Moments.Create(y);
        double v1 = xm.Variance, v2 = ym.Variance, m1 = xm.Mean, m2 = ym.Mean;

        threshold = threshold ?? RelativeThreshold.Default;
        double thresholdValue = threshold.Value(x);
        double se = Math.Sqrt(v1 / n1 + v2 / n2);
        double t = ((m1 - m2) - thresholdValue) / se;
        double df = (v1 / n1 + v2 / n2).Sqr() /
                    ((v1 / n1).Sqr() / (n1 - 1) + (v2 / n2).Sqr() / (n2 - 1));
        double pValue = 1 - new StudentDistribution(df).Cdf(t);

        return new WelchResult(t, df, pValue, threshold);
    }
}