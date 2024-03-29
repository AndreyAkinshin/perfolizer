using Perfolizer.Common;

namespace Perfolizer.Mathematics.Functions;

public static class FactorialFunction
{
    public static double Value(int n)
    {
        Assertion.NonNegative(nameof(n), n);

        if (n <= 20)
        {
            long value = 1;
            for (long i = 1; i <= n; i++)
                value *= i;
            return value;
        }

        return GammaFunction.Value(n + 1);
    }

    public static double LogValue(int n)
    {
        Assertion.NonNegative(nameof(n), n);

        if (n <= 20)
            return Log(Value(n));

        return GammaFunction.LogValue(n + 1);
    }

    public static double LogValue(double n)
    {
        Assertion.NonNegative(nameof(n), n);

        if (n <= 20)
        {
            int nRound = (int)Round(n);
            if (Abs(nRound - n) < 1e-9)
                return Log(Value(nRound));
        }

        return GammaFunction.LogValue(n + 1);
    }
}