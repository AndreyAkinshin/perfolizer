using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common;

internal static class MathExtensions
{
    public static double Sqr(this double x) => x * x;
    public static double Sqrt(this double x) => Math.Sqrt(x);
    public static double Pow(this int x, double k) => Math.Pow(x, k);
    public static double Pow(this double x, double k) => Math.Pow(x, k);
    public static double Clamp(this double x, double min, double max) => Min(Max(x, min), max);
    public static int Clamp(this int x, int min, int max) => Min(Max(x, min), max);
    public static int RoundToInt(this double x) => (int)Round(x);
    public static long RoundToLong(this double x) => (long)Round(x);

    public static long PowInt(this int x, int k) => PowLong(x, k);

    public static long PowLong(this long x, int k)
    {
        Assertion.NonNegative(nameof(k), k);
        long result = 1L;
        for (int i = 0; i < k; i++)
            result *= x;
        return result;
    }

    public static IEnumerable<double> Clamp(this IEnumerable<double> values, double min, double max)
        => values.Select(x => Clamp(x, min, max));
}