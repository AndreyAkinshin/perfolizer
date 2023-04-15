using System;
using Perfolizer.Common;
using Perfolizer.Mathematics.Functions;

namespace Perfolizer.Mathematics.Common;

public static class BinomialCoefficientHelper
{
    public const int MaxAcceptableN = 65;

    private static long[,]? pascalTriangle;

    public static long BinomialCoefficient(int n, int k)
    {
        const int maxN = MaxAcceptableN;
        if (n < 0 || n > maxN)
            throw new ArgumentOutOfRangeException(nameof(n));
        if (k < 0 || k > n)
            return 0;

        if (pascalTriangle == null)
        {
            checked
            {
                pascalTriangle = new long[maxN + 1, maxN + 1];
                for (int i = 0; i <= maxN; i++)
                {
                    pascalTriangle[i, 0] = 1;
                    for (int j = 1; j <= i; j++)
                        pascalTriangle[i, j] = pascalTriangle[i - 1, j - 1] + pascalTriangle[i - 1, j];
                }
            }
        }

        return pascalTriangle[n, k];
    }

    /// <summary>
    /// Log(C(n, k))
    /// </summary>
    public static double LogBinomialCoefficient(int n, int k)
    {
        Assertion.Positive(nameof(n), n);
        Assertion.InRangeInclusive(nameof(k), k, 0, n);
            
        return FactorialFunction.LogValue(n) - FactorialFunction.LogValue(k) - FactorialFunction.LogValue(n - k);
    }
}