using JetBrains.Annotations;
using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Metrology;

public readonly struct EffectSizeValue(double value) : IAbsoluteMeasurementValue
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
        var measurementValue = new Measurement(Value, EffectSizeUnit.Instance);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public MeasurementUnit Unit => EffectSizeUnit.Instance;

    public double GetShift(Sample sample) => ShamosEstimator.Instance.Scale(sample) * Value;
}