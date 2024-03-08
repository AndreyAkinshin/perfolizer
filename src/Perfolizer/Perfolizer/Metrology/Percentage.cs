using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Metrology;

public readonly struct Percentage(Probability probability)
{
    private const string DefaultFormat = "0.###";
    [PublicAPI] public Probability Probability { get; } = probability;

    public override string ToString() => ToString(null);

    [Pure, PublicAPI]
    public string ToString(
        string? format,
        CultureInfo? cultureInfo = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        double nominalValue = Probability * 100.0;
        var measurementValue = new MeasurementValue(nominalValue, PercentageUnit.Instance);
        return measurementValue.ToString(format, cultureInfo, unitPresentation);
    }
}