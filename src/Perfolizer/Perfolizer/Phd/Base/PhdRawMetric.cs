namespace Perfolizer.Phd.Base;

public class PhdRawMetric
{
    public string? Id { get; set; }
    public int? Version { get; set; }
    public int? IterationIndex { get; set; }
    public long? InvocationCount { get; set; }
    public long? StartTimestamp { get; set; }
    public long? EndTimestamp { get; set; }
    public string? Marker { get; set; }

    /// <summary>
    /// Examples: mean, median, min, max, p0, p25, p75, p100
    /// </summary>
    public string? Aggregator { get; set; }

    public double Value { get; set; }
    public string? Unit { get; set; }
}