using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Horology;

[PublicAPI]
public readonly struct Frequency(double hertz) : IEquatable<Frequency>, IComparable<Frequency>
{
    private const string DefaultFormat = "G";

    [PublicAPI] public double Hertz { get; } = hertz;

    [PublicAPI]
    public Frequency(double value, FrequencyUnit unit) : this(value * unit.BaseUnits)
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
    public static bool TryParse(string? s, FrequencyUnit unit, out Frequency freq)
    {
        return TryParse(s, unit, NumberStyles.Any, DefaultCultureInfo.Instance, out freq);
    }

    [PublicAPI]
    public static bool TryParse(string? s, FrequencyUnit unit, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
    {
        bool success = double.TryParse(s, numberStyle, formatProvider, out double result);
        freq = new Frequency(result, unit);
        return success;
    }

    [PublicAPI]
    public static bool TryParseHz(string? s, out Frequency freq) =>
        TryParse(s, FrequencyUnit.Hz, out freq);

    [PublicAPI]
    public static bool TryParseHz(string? s, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.Hz, numberStyle, formatProvider, out freq);

    [PublicAPI]
    public static bool TryParseKHz(string? s, out Frequency freq) =>
        TryParse(s, FrequencyUnit.KHz, out freq);

    [PublicAPI]
    public static bool TryParseKHz(string? s, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.KHz, numberStyle, formatProvider, out freq);

    [PublicAPI]
    public static bool TryParseMHz(string? s, out Frequency freq) =>
        TryParse(s, FrequencyUnit.MHz, out freq);

    [PublicAPI]
    public static bool TryParseMHz(string? s, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.MHz, numberStyle, formatProvider, out freq);

    [PublicAPI]
    public static bool TryParseGHz(string? s, out Frequency freq) =>
        TryParse(s, FrequencyUnit.GHz, out freq);

    [PublicAPI]
    public static bool TryParseGHz(string? s, NumberStyles numberStyle,
        IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.GHz, numberStyle, formatProvider, out freq);

    public double GetShift(Sample sample)
    {
        if (sample.Unit is not FrequencyUnit frequencyUnit)
            throw new InvalidMeasurementUnitExceptions(FrequencyUnit.Hz, sample.Unit);
        return FrequencyUnit.Convert(Hertz, FrequencyUnit.Hz, frequencyUnit);
    }

    public Measurement ToMeasurement(FrequencyUnit? frequencyUnit = null)
    {
        frequencyUnit ??= FrequencyUnit.GetBestFrequencyUnit(Hertz);
        double nominalValue = FrequencyUnit.Convert(Hertz, FrequencyUnit.Hz, frequencyUnit);
        return new Measurement(nominalValue, frequencyUnit);
    }

    public override string ToString() => MeasurementFormatter.Default.Format(ToMeasurement(), DefaultFormat);

    public bool Equals(Frequency other) => Hertz.Equals(other.Hertz);
    public bool Equals(Frequency other, double hertzEpsilon) => Abs(Hertz - other.Hertz) < hertzEpsilon;
    public override bool Equals(object? obj) => obj is Frequency other && Equals(other);
    public override int GetHashCode() => Hertz.GetHashCode();
    public int CompareTo(Frequency other) => Hertz.CompareTo(other.Hertz);
}