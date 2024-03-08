using Perfolizer.Extensions;
using Perfolizer.Metrology;

namespace Perfolizer.Phd.Base;

public class PhdMetric
{
    public string? Id { get; set; }
    public int? Version { get; set; }
    public int? IterationIndex { get; set; }
    public long? InvocationCount { get; set; }
    public DateTimeOffset? StartTimestamp { get; set; }
    public DateTimeOffset? EndTimestamp { get; set; }
    public string? Marker { get; set; }

    /// <summary>
    /// Examples: mean, median, min, max, p0, p25, p75, p100
    /// </summary>
    public string? Aggregator { get; set; }

    public double Value { get; set; }
    public MeasurementUnit Unit { get; set; } = NumberUnit.Instance;

    public Measurement GetMeasurementValue() => new(Value, Unit);
    public string GetIdentity() => $"{Id}§{Version}§{Aggregator}§{Marker}§{Unit}";

    public PhdRawMetric Serialize() => new()
    {
        Id = Id,
        Version = Version,
        IterationIndex = IterationIndex,
        InvocationCount = InvocationCount,
        StartTimestamp = StartTimestamp?.ToUnixTimeMilliseconds(),
        EndTimestamp = EndTimestamp?.ToUnixTimeMilliseconds(),
        Marker = Marker,
        Aggregator = Aggregator,
        Value = Value,
        Unit = Unit.AbbreviationAscii.IsBlank() ? null : Unit.AbbreviationAscii
    };

    public static PhdMetric Deserialize(PhdRawMetric phdRawMetric) => new()
    {
        Id = phdRawMetric.Id,
        Version = phdRawMetric.Version,
        IterationIndex = phdRawMetric.IterationIndex,
        InvocationCount = phdRawMetric.InvocationCount,
        StartTimestamp = phdRawMetric.StartTimestamp.HasValue
            ? DateTimeOffset.FromUnixTimeMilliseconds(phdRawMetric.StartTimestamp.Value)
            : null,
        EndTimestamp = phdRawMetric.EndTimestamp.HasValue
            ? DateTimeOffset.FromUnixTimeMilliseconds(phdRawMetric.EndTimestamp.Value)
            : null,
        Marker = phdRawMetric.Marker,
        Aggregator = phdRawMetric.Aggregator,
        Value = phdRawMetric.Value,
        Unit = MeasurementUnit.TryParse(phdRawMetric.Unit, out var unit) ? unit : NumberUnit.Instance
    };
}