using JetBrains.Annotations;
using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Metrology;

public readonly struct EffectSizeValue(double value) : IApplicableMeasurementUnit
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
        var measurementValue = new MeasurementValue(Value, EffectSizeUnit.Instance);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public Sample Apply(Sample sample)
    {
        double scale = ShamosEstimator.Instance.Scale(sample);
        double shift = scale * Value;
        return MeasurementValueHelper.Apply(sample, x => x + shift);
    }
}