using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Horology;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Metrology;

public class MeasurementValue(double nominalValue, MeasurementUnit unit)
{
    public static readonly MeasurementValue Zero = new(0, NumberUnit.Instance);

    protected virtual string DefaultFormat => "G";

    public double NominalValue { get; } = nominalValue;
    public MeasurementUnit Unit { get; } = unit;

    public TimeInterval? AsTimeInterval() =>
        Unit is TimeUnit timeUnit ? new TimeInterval(NominalValue, timeUnit) : null;

    public SizeValue? AsSizeValue() =>
        Unit is SizeUnit sizeUnit ? new SizeValue(NominalValue.RoundToLong(), sizeUnit) : null;

    public NumberValue? AsNumberValue() =>
        Unit is NumberUnit ? new NumberValue(NominalValue) : null;

    public Percentage? AsPercentage() =>
        Unit is PercentageUnit ? new Percentage(NominalValue / 100.0) : null;

    public EffectSizeValue? AsEffectSizeValue() =>
        Unit is EffectSizeUnit ? new EffectSizeValue(NominalValue) : null;

    public RatioValue? AsRatioValue() =>
        Unit is RatioUnit ? new RatioValue(NominalValue) : null;

    public override string ToString() =>
        AsTimeInterval()?.ToString() ??
        AsSizeValue()?.ToString() ??
        AsNumberValue()?.ToString() ??
        AsPercentage()?.ToString() ??
        AsEffectSizeValue()?.ToString() ??
        AsRatioValue()?.ToString() ??
        ToString(DefaultFormat);

    [Pure]
    public string ToString(
        string format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        formatProvider ??= DefaultCultureInfo.Instance;
        unitPresentation ??= UnitPresentation.Default;

        string unitValue = NominalValue.ToString(format, formatProvider);
        if (!unitPresentation.IsVisible)
            return unitValue;

        string abbreviation = unitPresentation.ForceAscii ? Unit.AbbreviationAscii : Unit.Abbreviation;
        string unitName = abbreviation.PadLeft(unitPresentation.MinUnitWidth);
        string gap = unitPresentation.Gap ? " " : "";
        return $"{unitValue}{gap}{unitName}";
    }

    public static bool TryParse(string s, out MeasurementValue value)
    {
        if (s.IsNotBlank())
        {
            foreach (var unit in MeasurementUnit.GetAll())
            {
                if (TryParse(s, unit, out value))
                    return true;
            }
        }

        value = new MeasurementValue(0, NumberUnit.Instance);
        return false;
    }

    public static bool TryParse(string s, MeasurementUnit unit, out MeasurementValue value)
    {
        if (TryParseBySuffix(unit.Abbreviation, out double nominalValue) ||
            TryParseBySuffix(unit.AbbreviationAscii, out nominalValue) ||
            TryParseBySuffix(unit.FullName, out nominalValue))
        {
            value = new MeasurementValue(nominalValue, unit);
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
}