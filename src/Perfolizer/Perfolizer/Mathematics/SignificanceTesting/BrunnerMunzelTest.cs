using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Metrology;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class BrunnerMunzelTest : SignificanceTwoSampleTestBase<BrunnerMunzelResult>
{
    private const double Eps = 1e-9;
    public static readonly BrunnerMunzelTest Instance = new();

    private BrunnerMunzelTest()
    {
    }

    public override BrunnerMunzelResult Perform(
        Sample x,
        Sample y,
        AlternativeHypothesis alternative,
        Threshold threshold)
    {
        Assertion.NotNullOrEmpty(nameof(x), x);
        Assertion.NotNullOrEmpty(nameof(y), y);

        int n = x.Count;
        int m = y.Count;

        var y2 = threshold.ApplyMax(y);
        double[] xy = new double[n + m];
        for (int i = 0; i < n; i++)
            xy[i] = x.Values[i];
        for (int i = 0; i < m; i++)
            xy[n + i] = y2.Values[i];

        double[] rx = Ranker.Instance.GetRanks(x.Values);
        double[] ry = Ranker.Instance.GetRanks(y.Values);
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

        double sigmaX2 = sx2 / m / m;
        double sigmaY2 = sy2 / n / n;
        double sigma2 = (n + m) * (sigmaX2 / n + sigmaY2 / m);
        if (Abs(sigma2) < Eps)
        {
            double diff = rxMean - ryMean;
            if (Abs(diff) < Eps)
                return Result(0.5, 0, double.NaN);
            double w = diff > 0 ? double.PositiveInfinity : double.NegativeInfinity;

            return alternative switch
            {
                AlternativeHypothesis.TwoSides => Result(0, w, double.NaN),
                AlternativeHypothesis.Less => Result(rxMean > ryMean ? 1 : 0, w, double.NaN),
                AlternativeHypothesis.Greater => Result(rxMean < ryMean ? 1 : 0, w, double.NaN),
                _ => throw new ArgumentOutOfRangeException(nameof(alternative), alternative, null)
            };
        }
        else
        {
            double w = (rxMean - ryMean) / Sqrt(sigma2 * (n + m));
            double df = (sx2 / m + sy2 / n).Sqr() / ((sx2 / m).Sqr() / (n - 1) + (sy2 / n).Sqr() / (m - 1));
            double cdf = new StudentDistribution(df).Cdf(w);
            double pValue = SignificanceTestHelper.CdfToPValue(cdf, alternative);

            return Result(pValue, w, df);
        }

        BrunnerMunzelResult Result(double pValueResult, double wResult, double dfResult) =>
            new(x, y, threshold, alternative, pValueResult, wResult, dfResult);
    }
}