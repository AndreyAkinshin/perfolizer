using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Dto;

public class PhdAttributes
{
    /// <summary>
    /// Service information (like the structure of the reports)
    /// </summary>
    public PhdMeta? Meta { get; set; }

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
    public PhdParameters? Parameters { get; set; }

    /// <summary>
    /// Benchmark descriptor
    /// </summary>
    public PhdBenchmark? Benchmark { get; set; }

    /// <summary>
    /// Stage of the benchmark lifecycle (e.g. Pilot/Warmup/Actual/etc., Overhead/Workload, etc.)
    /// </summary>
    public PhdLifecycle? Lifecycle { get; set; }

    public PhdEntry ToPerfEntry() => new (this);
}