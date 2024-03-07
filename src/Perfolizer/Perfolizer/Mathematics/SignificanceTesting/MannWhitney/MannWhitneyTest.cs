using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting.MannWhitney;

public class MannWhitneyTest : SignificanceTwoSampleTestBase<MannWhitneyResult>
{
    public static readonly MannWhitneyTest Instance = new();

    private static IMannWhitneyCdf GetCdfEstimator(MannWhitneyStrategy mannWhitneyStrategy, int n, int m) =>
        mannWhitneyStrategy switch
        {
            MannWhitneyStrategy.Auto => n + m <= BinomialCoefficientHelper.MaxAcceptableN // TODO: improve
                ? MannWhitneyLoefflerExactCdf.Instance
                : MannWhitneyEdgeworthApproxCdf.Instance,
            MannWhitneyStrategy.ClassicExact => MannWhitneyClassicExactCdf.Instance,
            MannWhitneyStrategy.LoefflerExact => MannWhitneyLoefflerExactCdf.Instance,
            MannWhitneyStrategy.NormalApprox => MannWhitneyNormalApproxCdf.Instance,
            MannWhitneyStrategy.EdgeworthApprox => MannWhitneyEdgeworthApproxCdf.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(mannWhitneyStrategy), mannWhitneyStrategy, null)
        };

    private static MannWhitneyResult PerformGreater(
        Sample x,
        Sample y,
        Threshold threshold,
        MannWhitneyStrategy strategy)
    {
        // TODO: support weighted samples, see https://aakinshin.net/tags/research-wmw/
        Assertion.NonWeighted(nameof(x), x);
        Assertion.NonWeighted(nameof(y), y);

        Assertion.NotNullOrEmpty(nameof(x), x);
        Assertion.NotNullOrEmpty(nameof(y), y);

        int n = x.Count, m = y.Count;
        double[] xy = new double[n + m];
        for (int i = 0; i < n; i++)
            xy[i] = x.Values[i];
        for (int i = 0; i < m; i++)
            xy[n + i] = y.Values[i];
        int[] index = new int[n + m];
        for (int i = 0; i < n + m; i++)
            index[i] = i;
        Array.Sort(index, (i, j) => xy[i].CompareTo(xy[j]));

        double[] ranks = new double[n + m];
        for (int i = 0; i < n + m;)
        {
            int j = i;
            while (j + 1 < n + m && Abs(xy[index[j + 1]] - xy[index[i]]) < 1e-9)
                j++;
            double rank = (i + j + 2) / 2.0;
            for (int k = i; k <= j; k++)
                ranks[k] = rank;
            i = j + 1;
        }

        double ux = 0;
        for (int i = 0; i < n + m; i++)
            if (index[i] < n)
                ux += ranks[i];
        ux -= n * (n + 1) / 2.0;
        double uy = n * m - ux;


        var cdfEstimator = GetCdfEstimator(strategy, n, m);
        double cdf = cdfEstimator.Cdf(n, m, ux.RoundToInt());
        double pValue = 1 - cdf;

        return new MannWhitneyResult(x, y, threshold, AlternativeHypothesis.Greater, pValue, ux, uy);
    }

    public override MannWhitneyResult Perform(
        Sample x,
        Sample y,
        AlternativeHypothesis alternative,
        Threshold threshold)
    {
        return PerformStrategy(x, y, alternative, threshold);
    }

    [PublicAPI]
    public MannWhitneyResult PerformStrategy(
        Sample x,
        Sample y,
        AlternativeHypothesis alternative,
        Threshold threshold,
        MannWhitneyStrategy mannWhitneyStrategy = MannWhitneyStrategy.Auto)
    {
        switch (alternative)
        {
            case AlternativeHypothesis.TwoSides:
            {
                var result1 = PerformGreater(x, threshold.Apply(y), threshold, mannWhitneyStrategy);
                var result2 = PerformGreater(threshold.Apply(y), x, threshold, mannWhitneyStrategy);
                double pValue = Min(Min(result1.PValue, result2.PValue) * 2, 1);
                return new MannWhitneyResult(x, y, threshold, alternative, pValue, result1.Ux, result1.Uy);
            }
            case AlternativeHypothesis.Less:
            {
                var result = PerformGreater(threshold.Apply(y), x, threshold, mannWhitneyStrategy);
                return new MannWhitneyResult(x, y, threshold, alternative, result.PValue, result.Uy, result.Ux);
            }
            case AlternativeHypothesis.Greater:
            {
                return PerformGreater(x, threshold.Apply(y), threshold, mannWhitneyStrategy);
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(alternative), alternative, null);
            }
        }
    }
}