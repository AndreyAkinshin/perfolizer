using Perfolizer.Phd.Dto;

namespace Perfolizer.Phd.Base;

public class PhdRawEntry
{
    public PhdAttributes Attributes { get; set; } = new();
    public List<PhdRawMetric> Metrics { get; set; } = new();
    public List<PhdRawEntry> Children { get; set; } = new();
}