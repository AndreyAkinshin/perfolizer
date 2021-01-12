using System;

namespace Perfolizer.Common
{
    internal static class RandomExtensions
    {
        public static double NextDouble(this Random random, double min, double max)
        {
            return min + random.NextDouble() * (max - min);
        }
    }
}