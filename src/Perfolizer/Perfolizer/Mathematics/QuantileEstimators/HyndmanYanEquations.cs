using System;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    internal static class HyndmanYanEquations
    {
        /// <summary>
        /// Returns 1-based real index estimation
        /// </summary>
        public static double GetH(HyndmanYanType type, double n, Probability p) => type switch
        {
            HyndmanYanType.Type1 => n * p + 0.5,
            HyndmanYanType.Type2 => n * p + 0.5,
            HyndmanYanType.Type3 => n * p,
            HyndmanYanType.Type4 => n * p,
            HyndmanYanType.Type5 => n * p + 0.5,
            HyndmanYanType.Type6 => (n + 1) * p,
            HyndmanYanType.Type7 => (n - 1) * p + 1,
            HyndmanYanType.Type8 => (n + 1.0 / 3) * p + 1.0 / 3,
            HyndmanYanType.Type9 => (n + 1.0 / 4) * p + 3.0 / 8,
            _ => throw new InvalidOperationException()
        };

        public static double Evaluate(HyndmanYanType type, int n, Probability p, Func<int, double> getValue)
        {
            double h = GetH(type, n, p);

            double LinearInterpolation()
            {
                int hFloor = (int) h;
                double fraction = h - hFloor;
                if (hFloor + 1 <= n)
                    return getValue(hFloor) * (1 - fraction) + getValue(hFloor + 1) * fraction;
                return getValue(hFloor);
            }

            return type switch
            {
                HyndmanYanType.Type1 => getValue((int) Math.Ceiling(h - 0.5)),
                HyndmanYanType.Type2 => (getValue((int) Math.Ceiling(h - 0.5)) + getValue((int) Math.Floor(h + 0.5))) / 2,
                HyndmanYanType.Type3 => getValue((int) Math.Round(h, MidpointRounding.ToEven)),
                _ => LinearInterpolation()
            };
        }
    }
}