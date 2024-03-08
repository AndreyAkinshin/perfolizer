using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Metrology;

namespace Perfolizer.Horology;

[PublicAPI]
public readonly struct Frequency : IEquatable<Frequency>, IComparable<Frequency>
{
    private const string DefaultFormat = "";

    [PublicAPI] public double Hertz { get; }

    [PublicAPI] public Frequency(double hertz) => Hertz = hertz;

    [PublicAPI] public Frequency(double value, FrequencyUnit unit) : this(value * unit.HertzAmount) { }

    [PublicAPI] public static readonly Frequency Zero = new(0);
    [PublicAPI] public static readonly Frequency Hz = FrequencyUnit.Hz.ToFrequency();
    [PublicAPI] public static readonly Frequency KHz = FrequencyUnit.KHz.ToFrequency();
    [PublicAPI] public static readonly Frequency MHz = FrequencyUnit.MHz.ToFrequency();
    [PublicAPI] public static readonly Frequency GHz = FrequencyUnit.GHz.ToFrequency();

    [PublicAPI, Pure] public TimeInterval ToResolution() => TimeInterval.Second / Hertz;

    [PublicAPI, Pure] public double ToHz() => this / Hz;
    [PublicAPI, Pure] public double ToKHz() => this / KHz;
    [PublicAPI, Pure] public double ToMHz() => this / MHz;
    [PublicAPI, Pure] public double ToGHz() => this / GHz;

    [PublicAPI, Pure] public static Frequency FromHz(double value) => Hz * value;
    [PublicAPI, Pure] public static Frequency FromKHz(double value) => KHz * value;
    [PublicAPI, Pure] public static Frequency FromMHz(double value) => MHz * value;
    [PublicAPI, Pure] public static Frequency FromGHz(double value) => GHz * value;

    [PublicAPI, Pure] public static implicit operator Frequency(double value) => new(value);
    [PublicAPI, Pure] public static implicit operator double(Frequency property) => property.Hertz;

    [PublicAPI, Pure] public static double operator /(Frequency a, Frequency b) => 1.0 * a.Hertz / b.Hertz;
    [PublicAPI, Pure] public static Frequency operator /(Frequency a, double k) => new(a.Hertz / k);
    [PublicAPI, Pure] public static Frequency operator /(Frequency a, int k) => new(a.Hertz / k);
    [PublicAPI, Pure] public static Frequency operator *(Frequency a, double k) => new(a.Hertz * k);
    [PublicAPI, Pure] public static Frequency operator *(Frequency a, int k) => new(a.Hertz * k);
    [PublicAPI, Pure] public static Frequency operator *(double k, Frequency a) => new(a.Hertz * k);
    [PublicAPI, Pure] public static Frequency operator *(int k, Frequency a) => new(a.Hertz * k);
    [PublicAPI, Pure] public static bool operator <(Frequency a, Frequency b) => a.Hertz < b.Hertz;
    [PublicAPI, Pure] public static bool operator >(Frequency a, Frequency b) => a.Hertz > b.Hertz;
    [PublicAPI, Pure] public static bool operator <=(Frequency a, Frequency b) => a.Hertz <= b.Hertz;
    [PublicAPI, Pure] public static bool operator >=(Frequency a, Frequency b) => a.Hertz >= b.Hertz;
    [PublicAPI, Pure] public static bool operator ==(Frequency a, Frequency b) => a.Hertz.Equals(b.Hertz);
    [PublicAPI, Pure] public static bool operator !=(Frequency a, Frequency b) => !a.Hertz.Equals(b.Hertz);

    [PublicAPI, Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, FrequencyUnit unit, out Frequency freq)
    {
        return TryParse(s, unit, NumberStyles.Any, DefaultCultureInfo.Instance, out freq);
    }

    [PublicAPI, Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, FrequencyUnit unit, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
    {
        bool success = double.TryParse(s, numberStyle, formatProvider, out double result);
        freq = new Frequency(result, unit);
        return success;
    }

    [PublicAPI, Pure] public static bool TryParseHz([NotNullWhen(true)] string? s, out Frequency freq) => TryParse(s, FrequencyUnit.Hz, out freq);
    [PublicAPI, Pure]
    public static bool TryParseHz([NotNullWhen(true)] string? s, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.Hz, numberStyle, formatProvider, out freq);

    [PublicAPI, Pure] public static bool TryParseKHz([NotNullWhen(true)] string? s, out Frequency freq) => TryParse(s, FrequencyUnit.KHz, out freq);
    [PublicAPI, Pure]
    public static bool TryParseKHz([NotNullWhen(true)] string? s, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.KHz, numberStyle, formatProvider, out freq);

    [PublicAPI, Pure] public static bool TryParseMHz([NotNullWhen(true)] string? s, out Frequency freq) => TryParse(s, FrequencyUnit.MHz, out freq);
    [PublicAPI, Pure]
    public static bool TryParseMHz([NotNullWhen(true)] string? s, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.MHz, numberStyle, formatProvider, out freq);

    [PublicAPI, Pure] public static bool TryParseGHz([NotNullWhen(true)] string? s, out Frequency freq) => TryParse(s, FrequencyUnit.GHz, out freq);
    [PublicAPI, Pure]
    public static bool TryParseGHz([NotNullWhen(true)] string? s, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.GHz, numberStyle, formatProvider, out freq);

    [PublicAPI, Pure]
    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        return ToString(null, format, formatProvider, unitPresentation);
    }

    [PublicAPI, Pure]
    public string ToString(
        FrequencyUnit? frequencyUnit,
        string? format = null,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        frequencyUnit ??= FrequencyUnit.GetBestFrequencyUnit(Hertz);
        format ??= DefaultFormat;
        formatProvider ??= DefaultCultureInfo.Instance;
        unitPresentation ??= UnitPresentation.Default;
        double unitValue = FrequencyUnit.Convert(Hertz, FrequencyUnit.Hz, frequencyUnit);
        if (unitPresentation.IsVisible)
        {
            string unitName = frequencyUnit.Name.PadLeft(unitPresentation.MinUnitWidth);
            return $"{unitValue.ToString(format, formatProvider)} {unitName}";
        }

        return unitValue.ToString(format, formatProvider);
    }

    [PublicAPI, Pure] public override string ToString() => ToString(DefaultFormat);

    public bool Equals(Frequency other) => Hertz.Equals(other.Hertz);
    public bool Equals(Frequency other, double hertzEpsilon) => Math.Abs(Hertz - other.Hertz) < hertzEpsilon;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Frequency other && Equals(other);
    public override int GetHashCode() => Hertz.GetHashCode();
    public int CompareTo(Frequency other) => Hertz.CompareTo(other.Hertz);

}