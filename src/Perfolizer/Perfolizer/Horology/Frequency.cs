using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Metrology;

namespace Perfolizer.Horology;

[PublicAPI]
public readonly struct Frequency(double hertz)
    : IEquatable<Frequency>, IComparable<Frequency>, IAbsoluteMeasurementValue
{
    private const string DefaultFormat = "G";

    [PublicAPI] public double Hertz { get; } = hertz;

    [PublicAPI] public Frequency(double value, FrequencyUnit unit) : this(value * unit.BaseUnits)
    {
    }

    [PublicAPI] public static readonly Frequency Zero = new(0);
    [PublicAPI] public static readonly Frequency Hz = FrequencyUnit.Hz.ToFrequency();
    [PublicAPI] public static readonly Frequency KHz = FrequencyUnit.KHz.ToFrequency();
    [PublicAPI] public static readonly Frequency MHz = FrequencyUnit.MHz.ToFrequency();
    [PublicAPI] public static readonly Frequency GHz = FrequencyUnit.GHz.ToFrequency();

    [PublicAPI] public TimeInterval ToResolution() => TimeInterval.Second / Hertz;

    [PublicAPI] public double ToHz() => this / Hz;
    [PublicAPI] public double ToKHz() => this / KHz;
    [PublicAPI] public double ToMHz() => this / MHz;
    [PublicAPI] public double ToGHz() => this / GHz;

    [PublicAPI] public static Frequency FromHz(double value) => Hz * value;
    [PublicAPI] public static Frequency FromKHz(double value) => KHz * value;
    [PublicAPI] public static Frequency FromMHz(double value) => MHz * value;
    [PublicAPI] public static Frequency FromGHz(double value) => GHz * value;

    [PublicAPI] public static implicit operator Frequency(double value) => new(value);
    [PublicAPI] public static implicit operator double(Frequency property) => property.Hertz;

    [PublicAPI] public static double operator /(Frequency a, Frequency b) => 1.0 * a.Hertz / b.Hertz;
    [PublicAPI] public static Frequency operator /(Frequency a, double k) => new(a.Hertz / k);
    [PublicAPI] public static Frequency operator /(Frequency a, int k) => new(a.Hertz / k);
    [PublicAPI] public static Frequency operator *(Frequency a, double k) => new(a.Hertz * k);
    [PublicAPI] public static Frequency operator *(Frequency a, int k) => new(a.Hertz * k);
    [PublicAPI] public static Frequency operator *(double k, Frequency a) => new(a.Hertz * k);
    [PublicAPI] public static Frequency operator *(int k, Frequency a) => new(a.Hertz * k);
    [PublicAPI] public static bool operator <(Frequency a, Frequency b) => a.Hertz < b.Hertz;
    [PublicAPI] public static bool operator >(Frequency a, Frequency b) => a.Hertz > b.Hertz;
    [PublicAPI] public static bool operator <=(Frequency a, Frequency b) => a.Hertz <= b.Hertz;
    [PublicAPI] public static bool operator >=(Frequency a, Frequency b) => a.Hertz >= b.Hertz;
    [PublicAPI] public static bool operator ==(Frequency a, Frequency b) => a.Hertz.Equals(b.Hertz);
    [PublicAPI] public static bool operator !=(Frequency a, Frequency b) => !a.Hertz.Equals(b.Hertz);

    [PublicAPI]
    public static bool TryParse([NotNullWhen(true)] string? s, FrequencyUnit unit, out Frequency freq)
    {
        return TryParse(s, unit, NumberStyles.Any, DefaultCultureInfo.Instance, out freq);
    }

    [PublicAPI]
    public static bool TryParse([NotNullWhen(true)] string? s, FrequencyUnit unit, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
    {
        bool success = double.TryParse(s, numberStyle, formatProvider, out double result);
        freq = new Frequency(result, unit);
        return success;
    }

    [PublicAPI] public static bool TryParseHz([NotNullWhen(true)] string? s, out Frequency freq) =>
        TryParse(s, FrequencyUnit.Hz, out freq);

    [PublicAPI]
    public static bool TryParseHz([NotNullWhen(true)] string? s, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.Hz, numberStyle, formatProvider, out freq);

    [PublicAPI] public static bool TryParseKHz([NotNullWhen(true)] string? s, out Frequency freq) =>
        TryParse(s, FrequencyUnit.KHz, out freq);

    [PublicAPI]
    public static bool TryParseKHz([NotNullWhen(true)] string? s, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.KHz, numberStyle, formatProvider, out freq);

    [PublicAPI] public static bool TryParseMHz([NotNullWhen(true)] string? s, out Frequency freq) =>
        TryParse(s, FrequencyUnit.MHz, out freq);

    [PublicAPI]
    public static bool TryParseMHz([NotNullWhen(true)] string? s, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.MHz, numberStyle, formatProvider, out freq);

    [PublicAPI] public static bool TryParseGHz([NotNullWhen(true)] string? s, out Frequency freq) =>
        TryParse(s, FrequencyUnit.GHz, out freq);

    [PublicAPI]
    public static bool TryParseGHz([NotNullWhen(true)] string? s, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.GHz, numberStyle, formatProvider, out freq);

    public MeasurementUnit Unit => FrequencyUnit.Hz;

    public double GetShift(Sample sample)
    {
        if (sample.Unit is not FrequencyUnit frequencyUnit)
            throw new InvalidMeasurementUnitExceptions(Unit, sample.Unit);
        return FrequencyUnit.Convert(Hertz, FrequencyUnit.Hz, frequencyUnit);
    }

    public override string ToString() => ToString(DefaultFormat);

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        return ToString(null, format, formatProvider, unitPresentation);
    }

    [PublicAPI]
    public string ToString(
        FrequencyUnit? frequencyUnit,
        string? format = null,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        frequencyUnit ??= FrequencyUnit.GetBestFrequencyUnit(Hertz);
        format ??= DefaultFormat;
        double nominalValue = FrequencyUnit.Convert(Hertz, FrequencyUnit.Hz, frequencyUnit);
        var measurementValue = new MeasurementValue(nominalValue, frequencyUnit);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public bool Equals(Frequency other) => Hertz.Equals(other.Hertz);
    public bool Equals(Frequency other, double hertzEpsilon) => Abs(Hertz - other.Hertz) < hertzEpsilon;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Frequency other && Equals(other);
    public override int GetHashCode() => Hertz.GetHashCode();
    public int CompareTo(Frequency other) => Hertz.CompareTo(other.Hertz);
}