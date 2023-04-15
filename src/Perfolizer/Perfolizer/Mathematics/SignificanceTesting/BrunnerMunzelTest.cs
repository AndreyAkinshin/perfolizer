using System;
using System.Linq;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class BrunnerMunzelTest : IOneSidedTest<BrunnerMunzelTestResult>
{
    public static readonly BrunnerMunzelTest Instance = new();

    private BrunnerMunzelTest()
    {
    }

    public BrunnerMunzelTestResult? IsGreater(double[] x, double[] y, Threshold? threshold = null)
    {
        if (x.Length <= 1 || y.Length <= 1)
            return null;

        threshold ??= RelativeThreshold.Default;
        double thresholdValue = threshold.Value(x);

        int n = x.Length;
        int m = y.Length;

        var xy = new double[n + m];
        for (int i = 0; i < n; i++)
            xy[i] = x[i];
        for (int i = 0; i < m; i++)
            xy[n + i] = y[i] + thresholdValue;

        double[] rx = Ranker.Instance.GetRanks(x);
        double[] ry = Ranker.Instance.GetRanks(y);
        double[] rxy = Ranker.Instance.GetRanks(xy);

        double rxMean = 0, ryMean = 0;
        for (int i = 0; i < n; i++)
            rxMean += rxy[i];
        for (int j = 0; j < m; j++)
            ryMean += rxy[n + j];
        rxMean /= n;
        ryMean /= m;

        double sx2 = 0, sy2 = 0;
        for (int i = 0; i < n; i++)
            sx2 += (rxy[i] - rx[i] - rxMean + (n + 1) / 2.0).Sqr();
        for (int j = 0; j < m; j++)
            sy2 += (rxy[n + j] - ry[j] - ryMean + (m + 1) / 2.0).Sqr();
        sx2 /= n - 1;
        sy2 /= m - 1;

        double sigmaX2 = sx2 / m / m, sigmaY2 = sy2 / n / n;
        double sigma2 = (n + m) * (sigmaX2 / n + sigmaY2 / m);
        if (Math.Abs(sigma2) < 1e-9)
        {
            return rxMean < ryMean
                ? new BrunnerMunzelTestResult(double.PositiveInfinity, double.NaN, 1, threshold)
                : new BrunnerMunzelTestResult(double.NegativeInfinity, double.NaN, 0, threshold);
        }

        double w = (ryMean - rxMean) / Math.Sqrt(sigma2 * (n + m));

        double df = (sx2 / m + sy2 / n).Sqr() / ((sx2 / m).Sqr() / (n - 1) + (sy2 / n).Sqr() / (m - 1));
        double pValue = new StudentDistribution(df).Cdf(w);
 
        return new BrunnerMunzelTestResult(w, df, pValue, threshold);
    }
}