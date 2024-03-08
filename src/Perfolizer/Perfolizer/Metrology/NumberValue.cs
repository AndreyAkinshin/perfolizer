using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public readonly struct NumberValue(double value) : IApplicableMeasurementUnit
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
        var measurementValue = new MeasurementValue(Value, NumberUnit.Instance);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public Sample? Apply(Sample sample)
    {
        if (sample.MeasurementUnit is not NumberUnit)
            return null;
        double shift = Value;
        return MeasurementValueHelper.Apply(sample, x => x + shift);
    }
}