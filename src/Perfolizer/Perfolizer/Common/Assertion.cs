using JetBrains.Annotations;
using Perfolizer.Exceptions;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Common;

internal static class Assertion
{
    [AssertionMethod]
    public static void NotNull(string name, object? value)
    {
        if (value == null)
            throw new ArgumentNullException(name, $"{name} can't be null");
    }

    [AssertionMethod]
    public static void NotNullOrEmpty<T>(string name, IReadOnlyList<T>? values)
    {
        if (values == null)
            throw new ArgumentNullException(name, $"{name} can't be null");
        if (values.Count == 0)
            throw new ArgumentException(name, $"{name} can't be empty");
    }

    [AssertionMethod]
    public static void NotNullOrEmpty(string name, Sample? sample)
    {
        if (sample == null)
            throw new ArgumentNullException(name, $"{name} can't be null");
        if (sample.Size == 0)
            throw new ArgumentException(name, $"{name} can't be empty");
    }

    [AssertionMethod]
    public static void ItemNotNull<T>(string name, IReadOnlyList<T> values)
    {
        for (int i = 0; i < values.Count; i++)
            if (values[i] is null)
                throw new ArgumentNullException($"{name}[{i}] is null, but {name} should not contain null items");
    }

    [AssertionMethod]
    public static void InRangeInclusive(string name, double value, double min, double max)
    {
        if (value < min || value > max)
        {
            string message = Format("{0}={1}, but it should be in range [{2};{3}]", name, value, min, max);
            throw new ArgumentOutOfRangeException(name, value, message);
        }
    }

    [AssertionMethod]
    public static void InRangeInclusive(string name, int value, int min, int max)
    {
        if (value < min || value > max)
        {
            string message = Format("{0}={1}, but it should be in range [{2};{3}]", name, value, min, max);
            throw new ArgumentOutOfRangeException(name, value, message);
        }
    }

    [AssertionMethod]
    public static void InRangeInclusive(string name, IReadOnlyList<double> values, double min, double max)
    {
        for (int i = 0; i < values.Count; i++)
        {
            double value = values[i];
            if (value < min || value > max)
            {
                string message = Format("{0}[{1}]={2}, but it should be in range [{3};{4}]", name, i, value, min, max);
                throw new ArgumentOutOfRangeException(name, value, message);
            }
        }
    }

    [AssertionMethod]
    public static void InRangeExclusive(string name, double value, double min, double max)
    {
        if (value <= min || value >= max)
        {
            string message = Format("{0}={1}, but it should be in range ({2};{3})", name, value, min, max);
            throw new ArgumentOutOfRangeException(name, value, message);
        }
    }

    [AssertionMethod]
    public static void Positive(string name, double value)
    {
        if (value <= 0)
        {
            string message = Format("{0}={1}, but it should be positive", name, value);
            throw new ArgumentOutOfRangeException(name, value, message);
        }
    }

    [AssertionMethod]
    public static void Positive(string name, int value)
    {
        if (value <= 0)
        {
            string message = Format("{0}={1}, but it should be positive", name, value);
            throw new ArgumentOutOfRangeException(name, value, message);
        }
    }

    [AssertionMethod]
    public static void Positive(string name, IReadOnlyList<double> values)
    {
        for (int i = 0; i < values.Count; i++)
        {
            double value = values[i];
            if (value <= 0)
            {
                string message = Format("{0}[{1}]={2}, but it should be positive", name, i, value);
                throw new ArgumentOutOfRangeException(name, value, message);
            }
        }
    }

    [AssertionMethod]
    public static void NonNegative(string name, double value)
    {
        if (value < 0)
        {
            string message = Format("{0}={1}, but it should be non-negative", name, value);
            throw new ArgumentOutOfRangeException(name, value, message);
        }
    }

    [AssertionMethod]
    public static void MoreThan(string name, double value, int threshold)
    {
        if (value <= threshold)
        {
            string message = Format("{0}={1}, but it should be more than {2}", name, value, threshold);
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

    [AssertionMethod]
    public static void SizeLargerThan(string name, Sample sample, int threshold)
    {
        if (sample.Size <= threshold)
        {
            string message = Format("{0}.Count={1}, but it should be more than {2}", name, sample.Size, threshold);
            throw new ArgumentOutOfRangeException(name, sample.Size, message);
        }
    }

    [AssertionMethod]
    public static void Equal(string name1, int value1, string name2, int value2)
    {
        if (value1 != value2)
        {
            string message = Format("{0}={1}, {2}={3}, but {0} and {2} should be equal", name1, value1, name2, value2);
            throw new ArgumentOutOfRangeException(name1, value1, message);
        }
    }

    [AssertionMethod]
    public static void Equal(MeasurementUnit unit1, MeasurementUnit unit2)
    {
        if (unit1 != unit2)
            throw new InvalidOperationException(
                $"Invalid operations on incompatible units: {unit1.FullName} and {unit2.FullName}");
    }

    [AssertionMethod]
    public static void Equal(string name, double value, double expectedValue, double eps = 1e-9)
    {
        if (Abs(value - expectedValue) > eps)
        {
            string message = Format("{0}={1}, but it should be equal to {2}", name, value, expectedValue);
            throw new ArgumentOutOfRangeException(name, value, message);
        }
    }

    [AssertionMethod]
    public static void NonWeighted(string name, Sample? sample)
    {
        NotNull(name, sample);
        if (sample is { IsWeighted: true })
            throw new WeightedSampleNotSupportedException(name);
    }

    [StringFormatMethod("format")]
    private static string Format(string format, params object[] args) =>
        string.Format(DefaultCultureInfo.Instance, format, args);
}