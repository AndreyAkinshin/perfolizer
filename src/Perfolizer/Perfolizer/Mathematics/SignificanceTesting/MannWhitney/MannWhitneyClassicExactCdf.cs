using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.SignificanceTesting.MannWhitney;

public class MannWhitneyClassicExactCdf : IMannWhitneyCdf
{
    public static readonly MannWhitneyClassicExactCdf Instance = new();

    public double Cdf(int n, int m, int u)
    {
        u -= 1;

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
}