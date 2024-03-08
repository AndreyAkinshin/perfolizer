using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public readonly struct PercentValue(double percentage) : IApplicableMeasurementUnit
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

    public Sample Apply(Sample sample)
    {
        double factor = 1.0 + Percentage / 100.0;
        return MeasurementValueHelper.Apply(sample, x => x * factor);
    }
}