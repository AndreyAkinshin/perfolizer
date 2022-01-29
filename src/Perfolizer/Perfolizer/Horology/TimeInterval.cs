using System;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Horology
{
    public readonly struct TimeInterval : IEquatable<TimeInterval>, IComparable<TimeInterval>
    {
        private const string DefaultFormat = "N4";
        
        public double Nanoseconds { get; }

        public TimeInterval(double nanoseconds) => Nanoseconds = nanoseconds;

        public TimeInterval(double value, TimeUnit unit) : this(value * unit.NanosecondAmount) { }

        public static readonly TimeInterval Nanosecond = TimeUnit.Nanosecond.ToInterval();
        public static readonly TimeInterval Microsecond = TimeUnit.Microsecond.ToInterval();
        public static readonly TimeInterval Millisecond = TimeUnit.Millisecond.ToInterval();
        public static readonly TimeInterval Second = TimeUnit.Second.ToInterval();
        public static readonly TimeInterval Minute = TimeUnit.Minute.ToInterval();
        public static readonly TimeInterval Hour = TimeUnit.Hour.ToInterval();
        public static readonly TimeInterval Day = TimeUnit.Day.ToInterval();

        [Pure] public Frequency ToFrequency() => new(Second / this);
        [Pure] public TimeInterval Abs() => new(Math.Abs(Nanoseconds));

        [Pure] public double ToNanoseconds() => this / Nanosecond;
        [Pure] public double ToMicroseconds() => this / Microsecond;
        [Pure] public double ToMilliseconds() => this / Millisecond;
        [Pure] public double ToSeconds() => this / Second;
        [Pure] public double ToMinutes() => this / Minute;
        [Pure] public double ToHours() => this / Hour;
        [Pure] public double ToDays() => this / Day;

        [Pure] public static TimeInterval FromNanoseconds(double value) => Nanosecond * value;
        [Pure] public static TimeInterval FromMicroseconds(double value) => Microsecond * value;
        [Pure] public static TimeInterval FromMilliseconds(double value) => Millisecond * value;
        [Pure] public static TimeInterval FromSeconds(double value) => Second * value;
        [Pure] public static TimeInterval FromMinutes(double value) => Minute * value;
        [Pure] public static TimeInterval FromHours(double value) => Hour * value;
        [Pure] public static TimeInterval FromDays(double value) => Day * value;

        [Pure] public static double operator /(TimeInterval a, TimeInterval b) => 1.0 * a.Nanoseconds / b.Nanoseconds;
        [Pure] public static TimeInterval operator /(TimeInterval a, double k) => new(a.Nanoseconds / k);
        [Pure] public static TimeInterval operator /(TimeInterval a, int k) => new(a.Nanoseconds / k);
        [Pure] public static TimeInterval operator *(TimeInterval a, double k) => new(a.Nanoseconds * k);
        [Pure] public static TimeInterval operator *(TimeInterval a, int k) => new(a.Nanoseconds * k);
        [Pure] public static TimeInterval operator *(double k, TimeInterval a) => new(a.Nanoseconds * k);
        [Pure] public static TimeInterval operator *(int k, TimeInterval a) => new(a.Nanoseconds * k);
        [Pure] public static bool operator <(TimeInterval a, TimeInterval b) => a.Nanoseconds < b.Nanoseconds;
        [Pure] public static bool operator >(TimeInterval a, TimeInterval b) => a.Nanoseconds > b.Nanoseconds;
        [Pure] public static bool operator <=(TimeInterval a, TimeInterval b) => a.Nanoseconds <= b.Nanoseconds;
        [Pure] public static bool operator >=(TimeInterval a, TimeInterval b) => a.Nanoseconds >= b.Nanoseconds;
        [Pure] public static bool operator ==(TimeInterval a, TimeInterval b) => a.Nanoseconds.Equals(b.Nanoseconds);
        [Pure] public static bool operator !=(TimeInterval a, TimeInterval b) => !a.Nanoseconds.Equals(b.Nanoseconds);

        [Pure]
        public string ToString(
            string? format,
            IFormatProvider? formatProvider = null,
            UnitPresentation? unitPresentation = null)
        {
            return ToString(null, format, formatProvider, unitPresentation);
        }

        [Pure]
        public string ToString(
            TimeUnit? timeUnit,
            string? format = null,
            IFormatProvider? formatProvider = null,
            UnitPresentation? unitPresentation = null)
        {
            timeUnit ??= TimeUnit.GetBestTimeUnit(Nanoseconds);
            format ??= DefaultFormat;
            formatProvider ??= DefaultCultureInfo.Instance;
            unitPresentation ??= UnitPresentation.Default;
            double unitValue = TimeUnit.Convert(Nanoseconds, TimeUnit.Nanosecond, timeUnit);
            if (unitPresentation.IsVisible)
            {
                string unitName = timeUnit.Name.PadLeft(unitPresentation.MinUnitWidth);
                return $"{unitValue.ToString(format, formatProvider)} {unitName}";
            }

            return unitValue.ToString(format, formatProvider);
        }
        
        [Pure] public override string ToString() => ToString(DefaultFormat);

        public int CompareTo(TimeInterval other) => Nanoseconds.CompareTo(other.Nanoseconds);
        public bool Equals(TimeInterval other) => Nanoseconds.Equals(other.Nanoseconds);
        public bool Equals(TimeInterval other, double nanosecondEpsilon) => Math.Abs(other.Nanoseconds - Nanoseconds) < nanosecondEpsilon;
        public override bool Equals(object obj) => obj is TimeInterval other && Equals(other);
        public override int GetHashCode() => Nanoseconds.GetHashCode();
    }
}