using System.Globalization;
using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public readonly struct EffectSizeValue(double value)
{
    private const string DefaultFormat = "G";
    [PublicAPI] public double Value { get; } = value;

    public override string ToString() => ToString(null);

    [Pure, PublicAPI]
    public string ToString(
        string? format,
        CultureInfo? cultureInfo = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        var measurementValue = new MeasurementValue(Value, EffectSizeUnit.Instance);
        return measurementValue.ToString(format, cultureInfo, unitPresentation);
    }
}