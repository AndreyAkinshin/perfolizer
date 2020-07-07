using System;
using JetBrains.Annotations;

namespace Perfolizer.Common
{
    internal static class Assertion
    {
        public static void InRangeInclusive(string name, double value, double min, double max)
        {
            if (value < min || value > max)
            {
                string message = Format("{0}={1}, but it should be in range [{2};{3}]", name, value, min, max);
                throw new ArgumentOutOfRangeException(name, value, message);
            }
        }
        
        public static void InRangeExclusive(string name, double value, double min, double max)
        {
            if (value <= min || value >= max)
            {
                string message = Format("{0}={1}, but it should be in range ({2};{3})", name, value, min, max);
                throw new ArgumentOutOfRangeException(name, value, message);
            }
        }

        public static void Positive(string name, double value)
        {
            if (value <= 0)
            {
                string message = Format("{0}={1}, but it should be positive", name, value);
                throw new ArgumentOutOfRangeException(name, value, message);
            }
        }
        
        [StringFormatMethod("format")]
        private static string Format(string format, params object[] args) => string.Format(DefaultCultureInfo.Instance, format, args);
    }
}