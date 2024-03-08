using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public readonly struct RatioValue(double value) : IRelativeMeasurementValue
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
        var measurementValue = new Measurement(Value, RatioUnit.Instance);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public MeasurementUnit Unit => RatioUnit.Instance;
    public double GetRatio() => Value;
}