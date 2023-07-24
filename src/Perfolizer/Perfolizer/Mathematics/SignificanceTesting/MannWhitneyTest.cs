using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class MannWhitneyTest : ISignificanceTwoSampleTest<MannWhitneyResult>
{
    public enum Strategy
    {
        Auto,
        Exact,
        NormalApproximation
    }

    public static readonly MannWhitneyTest Instance = new();

    private static double ExactCdfSmallN(int n, int m, double u)
    {
        int q = (int)Floor(u + 1e-9);
        int nm = Max(n, m);
        long[,,] w = new long[nm + 1, nm + 1, q + 1];
        for (int i = 0; i <= nm; i++)
        for (int j = 0; j <= nm; j++)
        for (int k = 0; k <= q; k++)
        {
            if (i == 0 || j == 0 || k == 0)
                w[i, j, k] = k == 0 ? 1 : 0;
            else if (k > i * j)
                w[i, j, k] = 0;
            else if (i > j)
                w[i, j, k] = w[j, i, k];
            else if (j > 0 && k < j)
                w[i, j, k] = w[i, k, k];
            else
                w[i, j, k] = w[i - 1, j, k - j] + w[i, j - 1, k];
        }

        long denominator = BinomialCoefficientHelper.BinomialCoefficient(n + m, m);
        long p = 0;
        if (q <= n * m / 2)
        {
            for (int i = 0; i <= q; i++)
                p += w[n, m, i];
        }
        else
        {
            q = n * m - q;
            for (int i = 0; i < q; i++)
                p += w[n, m, i];
            p = denominator - p;
        }

        return p * 1.0 / denominator;
    }

    private static double CdfNormalApproximation(double n, double m, double ux)
    {
        double mu = n * m / 2.0;
        double su = Sqrt(n * m * (n + m + 1) / 12.0);
        double z = (ux - mu) / su;
        return NormalDistribution.Gauss(z);
    }

    private static MannWhitneyResult RunGreater(Sample x, Sample y, Threshold threshold, Strategy strategy)
    {
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

        var ranks = new double[n + m];
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


        if (strategy == Strategy.Auto)
            strategy = n + m <= BinomialCoefficientHelper.MaxAcceptableN ? Strategy.Exact : Strategy.NormalApproximation; // TODO: improve
        
        double cdf = strategy switch
        {
            Strategy.Exact => ExactCdfSmallN(n, m, ux - 1), // TODO: support big N
            Strategy.NormalApproximation => CdfNormalApproximation(n, m, ux),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
        double pValue = 1 - cdf;
        
        return new MannWhitneyResult(x, y, threshold, AlternativeHypothesis.Greater, pValue, ux, uy);
    }

    public MannWhitneyResult Run(Sample x, Sample y,
        AlternativeHypothesis alternativeHypothesis = AlternativeHypothesis.Greater,
        Threshold? threshold = null) =>
        Run(x, y, alternativeHypothesis, threshold, Strategy.Auto);

    public MannWhitneyResult Run(Sample x, Sample y,
        AlternativeHypothesis alternativeHypothesis = AlternativeHypothesis.Greater,
        Threshold? threshold = null,
        Strategy strategy = Strategy.Auto)
    {
        // TODO: support weighted case
        Assertion.NonWeighted(nameof(x), x);
        Assertion.NonWeighted(nameof(y), y);

        threshold ??= AbsoluteThreshold.Zero;

        int n = x.Count, m = y.Count;
        if (Min(n, m) < 3 || Max(n, m) < 5) // TODO: adjust requirements
            throw new NotSupportedException("Samples are two small");

        switch (alternativeHypothesis)
        {
            case AlternativeHypothesis.TwoSides:
            {
                var result1 = RunGreater(x, threshold.Apply(y), threshold, strategy);
                var result2 = RunGreater(threshold.Apply(y), x, threshold, strategy);
                double pValue = Min(Min(result1.PValue, result2.PValue) * 2, 1);
                return new MannWhitneyResult(x, y, threshold, alternativeHypothesis, pValue, result1.Ux, result1.Uy);
            }
            case AlternativeHypothesis.Less:
            {
                var result = RunGreater(threshold.Apply(y), x, threshold, strategy);
                return new MannWhitneyResult(x, y, threshold, alternativeHypothesis, result.PValue, result.Uy, result.Ux);
            }
            case AlternativeHypothesis.Greater:
                return RunGreater(x, threshold.Apply(y), threshold, strategy);
            default:
                throw new ArgumentOutOfRangeException(nameof(alternativeHypothesis), alternativeHypothesis, null);
        }
    }
}