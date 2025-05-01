using Perfolizer.Collections;
using Perfolizer.Metrology;
using Perfolizer.Perfonar.Base;

namespace Perfolizer.Models;

public class EntryInfo : AbstractInfo
{
    /// <summary>
    /// Primary information about the current report
    /// </summary>
    public IdentityInfo? Identity { get; set; }

    /// <summary>
    /// Information about the measurement framework
    /// </summary>
    public EngineInfo? Engine { get; set; }

    /// <summary>
    /// BuildId, Commit, etc.
    /// </summary>
    public SourceInfo? Source { get; set; }

    /// <summary>
    /// Os, Cpu, etc.
    /// </summary>
    public HostInfo? Host { get; set; }

    /// <summary>
    /// Benchmark execution, environment, etc.
    /// </summary>
    public JobInfo? Job { get; set; }

    /// <summary>
    /// Benchmark parameters
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }

    /// <summary>
    /// Benchmark descriptor
    /// </summary>
    public BenchmarkInfo? Benchmark { get; set; }

    /// <summary>
    /// Stage of the benchmark lifecycle (e.g. Pilot/Warmup/Actual/etc., Overhead/Workload, etc.)
    /// </summary>
    public LifecycleInfo? Lifecycle { get; set; }

    public MetricInfo Metric { get; set; } = MetricInfo.Empty;
    public double? Value { get; set; }
    public MeasurementUnit Unit { get; set; } = NumberUnit.Instance;

    // TODO: Refactor
    public int? IterationIndex { get; set; }

    // TODO: Refactor
    public long? InvocationCount { get; set; }

    private readonly List<EntryInfo> nested = [];
    public IReadOnlyList<EntryInfo> Nested => nested;

    // Tree
    public EntryInfo Add(params EntryInfo[] entries)
    {
        foreach (var entry in entries)
            nested.Add(entry);
        return this;
    }

    public IEnumerable<EntryInfo> Traverse()
    {
        yield return this;
        foreach (var entry in nested.SelectMany(entry => entry.Traverse()))
            yield return entry;
    }
}