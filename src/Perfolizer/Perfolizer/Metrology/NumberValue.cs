using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public readonly struct NumberValue(double value) : IAbsoluteMeasurementValue
{
    private const string DefaultFormat = "G";
    [PublicAPI] public double Value { get; } = value;

    public static NumberValue Of(double value) => new(value);

    public override string ToString() => ToString(null);

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        var measurementValue = new Measurement(Value, NumberUnit.Instance);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public MeasurementUnit Unit => NumberUnit.Instance;
    public double GetShift(Sample sample) => Value;
}