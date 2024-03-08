using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Horology;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Metrology;

public class Measurement(double nominalValue, MeasurementUnit unit) : IWithUnits, IComparable<Measurement>, IComparable
{
    public static readonly Measurement Zero = new (0, NumberUnit.Instance);

    protected virtual string DefaultFormat => "G";

    public double NominalValue { get; } = nominalValue;
    public MeasurementUnit Unit { get; } = unit;

    public TimeInterval? AsTimeInterval() =>
        Unit is TimeUnit timeUnit ? new TimeInterval(NominalValue, timeUnit) : null;

    public SizeValue? AsSizeValue() =>
        Unit is SizeUnit sizeUnit ? new SizeValue(NominalValue.RoundToLong(), sizeUnit) : null;

    public Frequency? AsFrequency() =>
        Unit is FrequencyUnit frequencyUnit ? new Frequency(NominalValue, frequencyUnit) : null;

    public NumberValue? AsNumberValue() =>
        Unit is NumberUnit ? new NumberValue(NominalValue) : null;

    public PercentValue? AsPercentValue() =>
        Unit is PercentUnit ? new PercentValue(NominalValue) : null;

    public EffectSizeValue? AsEffectSizeValue() =>
        Unit is EffectSizeUnit ? new EffectSizeValue(NominalValue) : null;

    public RatioValue? AsRatioValue() =>
        Unit is RatioUnit ? new RatioValue(NominalValue) : null;

    public ISpecificMeasurementValue? AsApplicableMeasurementUnit()
    {
        return
            AsTimeInterval() ??
            AsSizeValue() ??
            AsFrequency() ??
            AsNumberValue() ??
            AsPercentValue() ??
            (ISpecificMeasurementValue?)AsEffectSizeValue() ??
            AsRatioValue();
    }


    public override string ToString() => AsApplicableMeasurementUnit()?.ToString() ?? ToString(DefaultFormat);

    [Pure]
    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        formatProvider ??= DefaultCultureInfo.Instance;
        unitPresentation ??= UnitPresentation.Default;

        string nominalValue = NominalValue.ToString(format, formatProvider);
        return $"{nominalValue}{Unit.ToString(unitPresentation)}";
    }

    public static bool TryParse(string s, out Measurement value)
    {
        if (s.IsNotBlank())
        {
            foreach (var unit in MeasurementUnit.GetAll())
            {
                if (TryParse(s, unit, out value))
                    return true;
            }
        }

        value = new Measurement(0, NumberUnit.Instance);
        return false;
    }

    public static Measurement Parse(string s) => TryParse(s, out var value) ? value : throw new FormatException($"Cannot parse {s} as a measurement");

    public static bool TryParse(string s, MeasurementUnit unit, out Measurement value)
    {
        if (TryParseBySuffix(unit.Abbreviation, out double nominalValue) ||
            TryParseBySuffix(unit.AbbreviationAscii, out nominalValue) ||
            TryParseBySuffix(unit.FullName, out nominalValue))
        {
            value = new Measurement(nominalValue, unit);
            return true;
        }

        value = Zero;
        return false;

        bool TryParseBySuffix(string suffix, out double value)
        {
            const NumberStyles numberStyles = NumberStyles.Float;
            var formatProvider = DefaultCultureInfo.Instance;
            if (s.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                string number = s.Substring(0, s.Length - suffix.Length).Trim();
                return double.TryParse(number, numberStyles, formatProvider, out value);
            }
            value = 0;
            return false;
        }
    }

    public static Measurement operator +(Measurement a, Measurement b)
    {
        Assertion.Equal(a.Unit, b.Unit);
        return new Measurement(a.NominalValue + b.NominalValue, a.Unit);
    }

    public static Measurement operator -(Measurement a, Measurement b)
    {
        Assertion.Equal(a.Unit, b.Unit);
        return new Measurement(a.NominalValue - b.NominalValue, a.Unit);
    }

    public int CompareTo(Measurement? other)
    {
        if (ReferenceEquals(this, other))
            return 0;
        if (ReferenceEquals(null, other))
            return 1;
        if (Unit.GetFlavor() != other.Unit.GetFlavor())
            throw new InvalidOperationException($"Cannot compare units of different flavors: {this} and {other}");
        double a = NominalValue * Unit.BaseUnits;
        double b = other.NominalValue * Unit.BaseUnits;
        return a.CompareTo(b);
    }

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        Measurement other => CompareTo(other),
        _ => throw new ArgumentException($"Object must be of type {nameof(Measurement)}")
    };

    public static bool operator <(Measurement? left, Measurement? right) =>
        Comparer<Measurement?>.Default.Compare(left, right) < 0;

    public static bool operator >(Measurement? left, Measurement? right) =>
        Comparer<Measurement?>.Default.Compare(left, right) > 0;

    public static bool operator <=(Measurement? left, Measurement? right) =>
        Comparer<Measurement?>.Default.Compare(left, right) <= 0;

    public static bool operator >=(Measurement? left, Measurement? right) =>
        Comparer<Measurement?>.Default.Compare(left, right) >= 0;
}