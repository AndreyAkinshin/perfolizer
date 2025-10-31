using Perfolizer.Horology;
using Perfolizer.Mathematics.Common;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

public static class PerfolizerMeasurementExtensions
{
    public static readonly Measurement Zero = new(0, NumberUnit.Instance);

    public static IEnumerable<MeasurementUnit> GetAll()
    {
        foreach (TimeUnit unit in TimeUnit.All)
            yield return unit;
        foreach (SizeUnit unit in SizeUnit.All)
            yield return unit;
        foreach (FrequencyUnit unit in FrequencyUnit.All)
            yield return unit;
        yield return NumberUnit.Instance;
        yield return PercentUnit.Instance;
        yield return RatioUnit.Instance;
        yield return DisparityUnit.Instance;
    }

    internal static Measurement WithUnit(this double value, MeasurementUnit unit) => new(value, unit);
    internal static Measurement WithUnitOf(this double value, Sample sample) => new(value, sample.Unit);

    public static TimeInterval? AsTimeInterval(this Measurement measurement) =>
        measurement.Unit is TimeUnit timeUnit ? new TimeInterval(measurement.NominalValue, timeUnit) : null;

    public static SizeValue? AsSizeValue(this Measurement measurement) =>
        measurement.Unit is SizeUnit sizeUnit ? new SizeValue(measurement.NominalValue.RoundToLong(), sizeUnit) : null;

    public static Frequency? AsFrequency(this Measurement measurement) =>
        measurement.Unit is FrequencyUnit frequencyUnit ? new Frequency(measurement.NominalValue, frequencyUnit) : null;

    public static NumberValue? AsNumberValue(this Measurement measurement) =>
        measurement.Unit is NumberUnit ? new NumberValue(measurement.NominalValue) : null;

    public static PercentValue? AsPercentValue(this Measurement measurement) =>
        measurement.Unit is PercentUnit ? new PercentValue(measurement.NominalValue) : null;

    public static DisparityValue? AsDisparityValue(this Measurement measurement) =>
        measurement.Unit is DisparityUnit ? new DisparityValue(measurement.NominalValue) : null;

    public static RatioValue? AsRatioValue(this Measurement measurement) =>
        measurement.Unit is RatioUnit ? new RatioValue(measurement.NominalValue) : null;

    public static Threshold ToThreshold(this NumberValue value) =>
        new(new Measurement(value.Value, NumberUnit.Instance));

    public static Threshold ToThreshold(this TimeInterval value) =>
        new(value.ToMeasurement());

    public static Threshold ToThreshold(this PercentValue value) =>
        new(value.ToMeasurement());
}