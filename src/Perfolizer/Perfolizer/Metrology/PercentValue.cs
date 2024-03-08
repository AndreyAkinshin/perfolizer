using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public readonly struct PercentValue(double actualValue) : IApplicableMeasurementUnit
{
    private const string DefaultFormat = "0.###";

    /// <summary>
    /// For 100%, it returns 1.0
    /// </summary>
    [PublicAPI] public double ActualValue { get; } = actualValue;

    public override string ToString() => ToString(null);

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        double nominalValue = ActualValue * 100.0;
        var measurementValue = new MeasurementValue(nominalValue, PercentUnit.Instance);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public Sample Apply(Sample sample)
    {
        double factor = 1.0 + ActualValue;
        return MeasurementValueHelper.Apply(sample, x => x * factor);
    }
}