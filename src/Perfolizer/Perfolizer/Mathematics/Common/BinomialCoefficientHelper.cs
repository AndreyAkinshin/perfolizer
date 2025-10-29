using Perfolizer.Common;
using Perfolizer.Mathematics.Functions;

namespace Perfolizer.Mathematics.Common;

public static class BinomialCoefficientHelper
{
    public const int MaxAcceptableN = 65;

    private static readonly Lazy<long[,]> PascalTriangle = new(BuildPascalTriangle);

    private static long[,] BuildPascalTriangle()
    {
        checked
        {
            long[,] triangle = new long[MaxAcceptableN + 1, MaxAcceptableN + 1];
            for (int i = 0; i <= MaxAcceptableN; i++)
            {
                triangle[i, 0] = 1;
                for (int j = 1; j <= i; j++)
                    triangle[i, j] = triangle[i - 1, j - 1] + triangle[i - 1, j];
            }

            return triangle;
        }
    }

    public static long BinomialCoefficient(int n, int k)
    {
        if (n < 0 || n > MaxAcceptableN)
            throw new ArgumentOutOfRangeException(nameof(n));
        if (k < 0 || k > n)
            return 0;

        return PascalTriangle.Value[n, k];
    }

    /// <summary>
    /// Log(C(n, k))
    /// </summary>
    public static double LogBinomialCoefficient(double n, double k)
    {
        Assertion.Positive(nameof(n), n);
        Assertion.InRangeInclusive(nameof(k), k, 0, n);

        return FactorialFunction.LogValue(n) - FactorialFunction.LogValue(k) - FactorialFunction.LogValue(n - k);
    }

    public static double BinomialCoefficient(double n, double k)
    {
        Assertion.Positive(nameof(n), n);
        Assertion.InRangeInclusive(nameof(k), k, 0, n);

        return Exp(LogBinomialCoefficient(n, k));
    }
}