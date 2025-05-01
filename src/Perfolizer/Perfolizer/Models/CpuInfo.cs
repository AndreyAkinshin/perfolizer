using JetBrains.Annotations;
using Perfolizer.Helpers;
using Perfolizer.Horology;

namespace Perfolizer.Models;

[PublicAPI]
public class CpuInfo : AbstractInfo
{
    public string? ProcessorName { get; set; }
    public int? PhysicalProcessorCount { get; set; }
    public int? PhysicalCoreCount { get; set; }
    public int? LogicalCoreCount { get; set; }
    public string? Architecture { get; set; }
    public long? NominalFrequencyHz { get; set; }
    public long? MaxFrequencyHz { get; set; }

    public Frequency? NominalFrequency() => NominalFrequencyHz.HasValue ? Frequency.FromHz(NominalFrequencyHz.Value) : null;
    public Frequency? MaxFrequency() => MaxFrequencyHz.HasValue ? Frequency.FromHz(MaxFrequencyHz.Value) : null;

    public override string GetDisplay() => Display ?? this.ToFullBrandName();
    public override string ToString() => this.ToFullBrandName();
}