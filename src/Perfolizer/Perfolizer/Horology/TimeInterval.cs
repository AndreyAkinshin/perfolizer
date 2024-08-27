using JetBrains.Annotations;
using Perfolizer.Exceptions;
using Perfolizer.Metrology;

namespace Perfolizer.Horology;

public readonly struct TimeInterval(double nanoseconds)
    : IEquatable<TimeInterval>, IComparable<TimeInterval>, IAbsoluteMeasurementValue
{
    private const string DefaultFormat = "0.###";

    public double Nanoseconds { get; } = nanoseconds;

    public TimeInterval(double value, TimeUnit unit) : this(value * unit.BaseUnits) { }

    [PublicAPI] public static readonly TimeInterval Zero = new (0);
    public static readonly TimeInterval Nanosecond = TimeUnit.Nanosecond.ToInterval();
    public static readonly TimeInterval Microsecond = TimeUnit.Microsecond.ToInterval();
    public static readonly TimeInterval Millisecond = TimeUnit.Millisecond.ToInterval();
    public static readonly TimeInterval Second = TimeUnit.Second.ToInterval();
    public static readonly TimeInterval Minute = TimeUnit.Minute.ToInterval();
    public static readonly TimeInterval Hour = TimeUnit.Hour.ToInterval();
    public static readonly TimeInterval Day = TimeUnit.Day.ToInterval();

    public Frequency ToFrequency() => new (Second / this);
    public TimeInterval Abs() => new (Math.Abs(Nanoseconds));

    public double ToNanoseconds() => this / Nanosecond;
    public double ToMicroseconds() => this / Microsecond;
    public double ToMilliseconds() => this / Millisecond;
    public double ToSeconds() => this / Second;
    public double ToMinutes() => this / Minute;
    public double ToHours() => this / Hour;
    public double ToDays() => this / Day;
    public double ToUnit(TimeUnit unit) => Nanoseconds / unit.BaseUnits;

    public static TimeInterval FromNanoseconds(double value) => Nanosecond * value;
    public static TimeInterval FromMicroseconds(double value) => Microsecond * value;
    public static TimeInterval FromMilliseconds(double value) => Millisecond * value;
    public static TimeInterval FromSeconds(double value) => Second * value;
    public static TimeInterval FromMinutes(double value) => Minute * value;
    public static TimeInterval FromHours(double value) => Hour * value;
    public static TimeInterval FromDays(double value) => Day * value;

    public static double operator /(TimeInterval a, TimeInterval b) => 1.0 * a.Nanoseconds / b.Nanoseconds;
    public static TimeInterval operator /(TimeInterval a, double k) => new (a.Nanoseconds / k);
    public static TimeInterval operator /(TimeInterval a, int k) => new (a.Nanoseconds / k);
    public static TimeInterval operator *(TimeInterval a, double k) => new (a.Nanoseconds * k);
    public static TimeInterval operator *(TimeInterval a, int k) => new (a.Nanoseconds * k);
    public static TimeInterval operator *(double k, TimeInterval a) => new (a.Nanoseconds * k);
    public static TimeInterval operator *(int k, TimeInterval a) => new (a.Nanoseconds * k);
    public static bool operator <(TimeInterval a, TimeInterval b) => a.Nanoseconds < b.Nanoseconds;
    public static bool operator >(TimeInterval a, TimeInterval b) => a.Nanoseconds > b.Nanoseconds;
    public static bool operator <=(TimeInterval a, TimeInterval b) => a.Nanoseconds <= b.Nanoseconds;
    public static bool operator >=(TimeInterval a, TimeInterval b) => a.Nanoseconds >= b.Nanoseconds;
    public static bool operator ==(TimeInterval a, TimeInterval b) => a.Nanoseconds.Equals(b.Nanoseconds);
    public static bool operator !=(TimeInterval a, TimeInterval b) => !a.Nanoseconds.Equals(b.Nanoseconds);

    public override string ToString() => ToString(null, null, null);

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        return ToString(null, format, formatProvider, unitPresentation);
    }

    public string ToString(
        TimeUnit? timeUnit,
        string? format = null,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        timeUnit ??= TimeUnit.GetBestTimeUnit(Nanoseconds);
        format ??= DefaultFormat;
        double nominalValue = TimeUnit.Convert(Nanoseconds, TimeUnit.Nanosecond, timeUnit);
        var measurementValue = new Measurement(nominalValue, timeUnit);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public int CompareTo(TimeInterval other) => Nanoseconds.CompareTo(other.Nanoseconds);
    public bool Equals(TimeInterval other) => Nanoseconds.Equals(other.Nanoseconds);

    public bool Equals(TimeInterval other, double nanosecondEpsilon) =>
        Math.Abs(other.Nanoseconds - Nanoseconds) < nanosecondEpsilon;

    public override bool Equals(object? obj) => obj is TimeInterval other && Equals(other);
    public override int GetHashCode() => Nanoseconds.GetHashCode();

    public MeasurementUnit Unit => TimeUnit.Nanosecond;

    public double GetShift(Sample sample)
    {
        if (sample.Unit is not TimeUnit timeUnit)
            throw new InvalidMeasurementUnitExceptions(Unit, sample.Unit);
        return TimeUnit.Convert(Nanoseconds, TimeUnit.Nanosecond, timeUnit);
    }
}