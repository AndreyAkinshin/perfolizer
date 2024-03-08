using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public readonly struct PercentValue(double percentage) : IRelativeMeasurementValue
{
    private const string DefaultFormat = "0.###";

    [PublicAPI] public double Percentage { get; } = percentage;
    [PublicAPI] public static PercentValue Of(double percentage) => new(percentage);

    public override string ToString() => ToString(null);

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        var measurementValue = new MeasurementValue(Percentage, PercentUnit.Instance);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public MeasurementUnit Unit => PercentUnit.Instance;
    public double GetRatio() => 1.0 + Percentage / 100.0;
}