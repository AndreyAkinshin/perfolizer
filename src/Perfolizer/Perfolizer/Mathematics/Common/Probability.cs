using System.Diagnostics.CodeAnalysis;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common;

public readonly struct Probability : IComparable, IComparable<Probability>, IEquatable<Probability>, IFormattable
{
    public static readonly Probability Zero = 0.0;
    public static readonly Probability Half = 0.5;
    public static readonly Probability Median = 0.5;
    public static readonly Probability One = 1.0;
    public static readonly Probability NaN = double.NaN;

    public readonly double Value;

    public Probability(double value)
    {
        Assertion.InRangeInclusive(nameof(value), value, 0, 1);
        Value = value;
    }

    public static Probability Of(double value) => new(value);

    public static implicit operator double(Probability probability) => probability.Value;
    public static implicit operator Probability(double value) => new(value);

    public override string ToString()
    {
        return Value.ToString(DefaultCultureInfo.Instance);
    }

    public int CompareTo(object obj)
    {
        return Value.CompareTo(obj);
    }

    public string ToString(IFormatProvider formatProvider)
    {
        return Value.ToString(formatProvider);
    }

    public string ToString(string format, IFormatProvider? formatProvider = null)
    {
        return Value.ToString(format, formatProvider ?? DefaultCultureInfo.Instance);
    }

    public bool Equals(Probability other)
    {
        return Value.Equals(other.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Probability other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public int CompareTo(Probability other)
    {
        return Value.CompareTo(other.Value);
    }

    [return: NotNullIfNotNull("values")]
    public static Probability[]? ToProbabilities(double[]? values)
    {
        if (values == null)
            return null;
        var probabilities = new Probability[values.Length];
        for (int i = 0; i < values.Length; i++)
            probabilities[i] = values[i];
        return probabilities;
    }
}