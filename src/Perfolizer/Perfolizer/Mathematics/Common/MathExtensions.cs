namespace Perfolizer.Mathematics.Common;

internal static class MathExtensions
{
    public static double Sqr(this double x) => x * x;
    public static double Sqrt(this double x) => Math.Sqrt(x);
    public static double Pow(this int x, double k) => Math.Pow(x, k);
    public static double Pow(this double x, double k) => Math.Pow(x, k);
    public static double Clamp(this double x, double min, double max) => Math.Min(Math.Max(x, min), max);
    public static int Clamp(this int x, int min, int max) => Math.Min(Math.Max(x, min), max);
    public static int RoundToInt(this double x) => (int)Round(x);

    public static IEnumerable<double> Clamp(this IEnumerable<double> values, double min, double max)
        => values.Select(x => Clamp(x, min, max));
}