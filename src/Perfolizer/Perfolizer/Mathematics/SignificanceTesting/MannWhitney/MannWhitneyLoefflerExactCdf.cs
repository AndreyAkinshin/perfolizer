using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.SignificanceTesting.MannWhitney;

/// <summary>
/// https://aakinshin.net/posts/mw-loeffler/
/// </summary>
public class MannWhitneyLoefflerExactCdf : IMannWhitneyCdf
{
    public static readonly MannWhitneyLoefflerExactCdf Instance = new();

    public double Cdf(int n, int m, int u)
    {
        return SumCdf(n, m, u) * 1.0 / BinomialCoefficientHelper.BinomialCoefficient(n + m, m);
    }

    private long SumCdf(int n, int m, int u) => u <= 0 ? 0 : FullCdf(n, m, u).Sum();

    // TODO: support big numbers
    // TODO: research the maximum values of n and m that we can handle
    private long[] FullCdf(int n, int m, int u)
    {
        u -= 1;

        int[] sigma = new int[u + 1];
        for (int d = 1; d <= n; d++)
            for (int i = d; i <= u; i += d)
                sigma[i] += d;
        for (int d = m + 1; d <= m + n; d++)
            for (int i = d; i <= u; i += d)
                sigma[i] -= d;

        long[] p = new long[u + 1];
        p[0] = 1;
        for (int a = 1; a <= u; a++)
        {
            for (int i = 0; i < a; i++)
                p[a] += p[i] * sigma[a - i];
            p[a] /= a;
        }

        return p;
    }
}