using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common;

public readonly struct SignificanceLevel
{
    [PublicAPI]
    public readonly double Value;

    [PublicAPI]
    public SignificanceLevel(double value)
    {
        Assertion.InRangeExclusive(nameof(value), value, 0, 1);
        Value = value;
    }

    public static implicit operator double(SignificanceLevel level) => level.Value;
    public static implicit operator SignificanceLevel(double value) => new(value);

    public override string ToString() => Value.ToString(DefaultCultureInfo.Instance);

    [PublicAPI]
    public string ToString(string format, IFormatProvider? formatProvider = null)
        => Value.ToString(format, formatProvider ?? DefaultCultureInfo.Instance);

    [PublicAPI] public static readonly SignificanceLevel P05 = 0.05;
    [PublicAPI] public static readonly SignificanceLevel P01 = 0.01;
    [PublicAPI] public static readonly SignificanceLevel P005 = 0.005;
    [PublicAPI] public static readonly SignificanceLevel P001 = 0.001;
    [PublicAPI] public static readonly SignificanceLevel P0005 = 0.0005;
    [PublicAPI] public static readonly SignificanceLevel P0001 = 0.0001;
    [PublicAPI] public static readonly SignificanceLevel P1E5 = 1e-5;
    [PublicAPI] public static readonly SignificanceLevel P1E6 = 1e-6;
    [PublicAPI] public static readonly SignificanceLevel P1E7 = 1e-7;
    [PublicAPI] public static readonly SignificanceLevel P1E8 = 1e-8;
    [PublicAPI] public static readonly SignificanceLevel P1E9 = 1e-9;
}