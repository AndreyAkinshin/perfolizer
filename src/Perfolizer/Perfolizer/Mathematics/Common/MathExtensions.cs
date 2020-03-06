using System;

namespace Perfolizer.Mathematics.Common
{
    internal static class MathExtensions
    {
        public static double Sqr(this double x) => x * x;
        public static double Pow(this double x, double k) => Math.Pow(x, k);
        public static double Clamp(this double x, double min, double max) => Math.Min(Math.Max(x, min), max);
    }
}