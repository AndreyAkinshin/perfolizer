using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Common
{
    internal static class Assertion
    {
        [AssertionMethod]
        public static void NotNull([NotNull] string name, [CanBeNull] object value)
        {
            if (value == null)
                throw new ArgumentNullException(name, $"{name} can't be null");
        }

        [AssertionMethod]
        public static void NotNullOrEmpty<T>([NotNull] string name, [CanBeNull] IReadOnlyList<T> values)
        {
            if (values == null)
                throw new ArgumentNullException(name, $"{name} can't be null");
            if (values.Count == 0)
                throw new ArgumentException(name, $"{name} can't be empty");
        }
        
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
        
        public static void NonNegative(string name, double value)
        {
            if (value < 0)
            {
                string message = Format("{0}={1}, but it should be non-negative", name, value);
                throw new ArgumentOutOfRangeException(name, value, message);
            }
        }

        [AssertionMethod]
        public static void MoreThan(string name, int value, int threshold)
        {
            if (value <= threshold)
            {
                string message = Format("{0}={1}, but it should be more than {2}", name, value, threshold);
                throw new ArgumentOutOfRangeException(name, value, message);
            }
        }

        [StringFormatMethod("format")]
        private static string Format(string format, params object[] args) => string.Format(DefaultCultureInfo.Instance, format, args);
    }
}