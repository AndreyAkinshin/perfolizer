using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public readonly struct RatioValue(double value) : IApplicableMeasurementUnit
{
    private const string DefaultFormat = "G";
    [PublicAPI] public double Value { get; } = value;

    public override string ToString() => ToString(null);

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        var measurementValue = new MeasurementValue(Value, RatioUnit.Instance);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public Sample Apply(Sample sample)
    {
        double factor = Value;
        return MeasurementValueHelper.Apply(sample, x => x * factor);
    }
}