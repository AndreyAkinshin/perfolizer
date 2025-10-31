using Pragmastat;

namespace Perfolizer.Mathematics.QuantileEstimators;

internal static class HyndmanFanHelper
{
    public static readonly HyndmanFanType[] AllTypes =
    {
        HyndmanFanType.Type1, HyndmanFanType.Type2, HyndmanFanType.Type3,
        HyndmanFanType.Type4, HyndmanFanType.Type5, HyndmanFanType.Type6,
        HyndmanFanType.Type7, HyndmanFanType.Type8, HyndmanFanType.Type9,
    };

    /// <summary>
    /// Returns 1-based real index estimation
    /// </summary>
    public static double GetH(HyndmanFanType type, double n, Probability p) => type switch
    {
        HyndmanFanType.Type1 => n * p + 0.5,
        HyndmanFanType.Type2 => n * p + 0.5,
        HyndmanFanType.Type3 => n * p,
        HyndmanFanType.Type4 => n * p,
        HyndmanFanType.Type5 => n * p + 0.5,
        HyndmanFanType.Type6 => (n + 1) * p,
        HyndmanFanType.Type7 => (n - 1) * p + 1,
        HyndmanFanType.Type8 => (n + 1.0 / 3) * p + 1.0 / 3,
        HyndmanFanType.Type9 => (n + 1.0 / 4) * p + 3.0 / 8,
        _ => throw new InvalidOperationException()
    };

    public static double Evaluate(HyndmanFanType type, int n, Probability p, Func<int, double> getValue)
    {
        double h = GetH(type, n, p);

        double LinearInterpolation()
        {
            int hFloor = (int)h;
            double fraction = h - hFloor;
            if (hFloor + 1 <= n)
                return getValue(hFloor) * (1 - fraction) + getValue(hFloor + 1) * fraction;
            return getValue(hFloor);
        }

        return type switch
        {
            HyndmanFanType.Type1 => getValue((int)Math.Ceiling(h - 0.5)),
            HyndmanFanType.Type2 => (getValue((int)Math.Ceiling(h - 0.5)) + getValue((int)Math.Floor(h + 0.5))) / 2,
            HyndmanFanType.Type3 => getValue((int)Math.Round(h, MidpointRounding.ToEven)),
            _ => LinearInterpolation()
        };
    }

    public static bool SupportsWeightedSamples(HyndmanFanType type) => type switch
    {
        HyndmanFanType.Type1 => false,
        HyndmanFanType.Type2 => false,
        HyndmanFanType.Type3 => false,
        HyndmanFanType.Type4 => true,
        HyndmanFanType.Type5 => true,
        HyndmanFanType.Type6 => true,
        HyndmanFanType.Type7 => true,
        HyndmanFanType.Type8 => true,
        HyndmanFanType.Type9 => true,
        _ => throw new InvalidOperationException()
    };
}