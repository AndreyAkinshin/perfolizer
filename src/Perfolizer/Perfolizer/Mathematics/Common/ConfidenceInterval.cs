using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common;

public readonly struct ConfidenceInterval : IEquatable<ConfidenceInterval>
{
    private const string DefaultFormat = "N2";

    public double Estimation { get; }
    public double Lower { get; }
    public double Upper { get; }
    public ConfidenceLevel ConfidenceLevel { get; }

    public ConfidenceInterval(double estimation, double lower, double upper, ConfidenceLevel confidenceLevel)
    {
        Estimation = estimation;
        Lower = lower;
        Upper = upper;
        ConfidenceLevel = confidenceLevel;
    }

    public bool Contains(double value, double eps = 1e-9) => Lower - eps < value && value < Upper + eps;

    public override string ToString() => ToString(DefaultFormat);

    public string ToString(string format, IFormatProvider? formatProvider = null, bool showLevel = true)
    {
        formatProvider ??= DefaultCultureInfo.Instance;

        var builder = new StringBuilder();
        builder.Append('[');
        builder.Append(Lower.ToString(format, formatProvider));
        builder.Append("; ");
        builder.Append(Upper.ToString(format, formatProvider));
        builder.Append("]");
        if (showLevel)
        {
            builder.Append(" (CI ");
            builder.Append(ConfidenceLevel.ToString());
            builder.Append(")");
        }

        return builder.ToString();
    }


    public bool Equals(ConfidenceInterval other)
    {
        return Estimation.Equals(other.Estimation) &&
               Lower.Equals(other.Lower) &&
               Upper.Equals(other.Upper) &&
               ConfidenceLevel.Equals(other.ConfidenceLevel);
    }

    public bool Equals(ConfidenceInterval other, IEqualityComparer<double> comparer)
    {
        return comparer.Equals(Estimation, other.Estimation) &&
               comparer.Equals(Lower, other.Lower) &&
               comparer.Equals(Upper, other.Upper) &&
               comparer.Equals(ConfidenceLevel, other.ConfidenceLevel);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ConfidenceInterval other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Estimation.GetHashCode();
            hashCode = (hashCode * 397) ^ Lower.GetHashCode();
            hashCode = (hashCode * 397) ^ Upper.GetHashCode();
            hashCode = (hashCode * 397) ^ ConfidenceLevel.GetHashCode();
            return hashCode;
        }
    }
}