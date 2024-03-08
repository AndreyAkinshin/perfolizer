using Perfolizer.Collections;
using Perfolizer.Metrology;
using Perfolizer.Phd.Dto;

namespace Perfolizer.Phd.Base;

public class PhdEntry : PhdObject
{
    /// <summary>
    /// Primary information about the current report
    /// </summary>
    public PhdInfo? Info { get; set; }

    /// <summary>
    /// Information about the measurement framework
    /// </summary>
    public PhdEngine? Engine { get; set; }

    /// <summary>
    /// BuildId, Commit, etc.
    /// </summary>
    public PhdSource? Source { get; set; }

    /// <summary>
    /// Os, Cpu, etc.
    /// </summary>
    public PhdHost? Host { get; set; }

    /// <summary>
    /// Benchmark execution, environment, etc.
    /// </summary>
    public PhdJob? Job { get; set; }

    /// <summary>
    /// Benchmark parameters
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }

    /// <summary>
    /// Benchmark descriptor
    /// </summary>
    public PhdBenchmark? Benchmark { get; set; }

    /// <summary>
    /// Stage of the benchmark lifecycle (e.g. Pilot/Warmup/Actual/etc., Overhead/Workload, etc.)
    /// </summary>
    public PhdLifecycle? Lifecycle { get; set; }

    public PhdMetric Metric { get; set; } = PhdMetric.Empty;
    public double? Value { get; set; }
    public MeasurementUnit Unit { get; set; } = NumberUnit.Instance;

    // TODO: Refactor
    public int? IterationIndex { get; set; }

    // TODO: Refactor
    public long? InvocationCount { get; set; }

    private readonly List<PhdEntry> nested = [];
    public IReadOnlyList<PhdEntry> Nested => nested;

    /// <summary>
    /// Service information (like the structure of the reports)
    /// </summary>
    public PhdMeta? Meta { get; set; }


    public PhdMeta? ResolveMeta()
    {
        if (Meta != null)
            return Meta;
        return Nested.Select(x => x.ResolveMeta()).WhereNotNull().FirstOrDefault();
    }

    // Tree
    public PhdEntry Add(params PhdEntry[] entries)
    {
        foreach (var entry in entries)
            nested.Add(entry);
        return this;
    }

    public IEnumerable<PhdEntry> Traverse()
    {
        yield return this;
        foreach (var entry in nested.SelectMany(entry => entry.Traverse()))
            yield return entry;
    }
}