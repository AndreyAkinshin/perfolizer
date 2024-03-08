using JetBrains.Annotations;
using Perfolizer.Helpers;
using Perfolizer.Horology;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Dto;

[PublicAPI]
public class PhdCpu : PhdObject
{
    public string? ProcessorName { get; set; }
    public int? PhysicalProcessorCount { get; set; }
    public int? PhysicalCoreCount { get; set; }
    public int? LogicalCoreCount { get; set; }
    public string? Architecture { get; set; }
    public long? NominalFrequencyHz { get; set; }
    public long? MinFrequencyHz { get; set; }
    public long? MaxFrequencyHz { get; set; }

    public Frequency? GetNominalFrequency() => NominalFrequencyHz.HasValue ? Frequency.FromHz(NominalFrequencyHz.Value) : null;
    public Frequency? GetMinFrequency() => MinFrequencyHz.HasValue ? Frequency.FromHz(MinFrequencyHz.Value) : null;
    public Frequency? GetMaxFrequency() => MaxFrequencyHz.HasValue ? Frequency.FromHz(MaxFrequencyHz.Value) : null;

    public PhdCpu SetDisplay()
    {
        Display = this.ToFullBrandName();
        return this;
    }

    public override string ToString() => this.ToFullBrandName();
}