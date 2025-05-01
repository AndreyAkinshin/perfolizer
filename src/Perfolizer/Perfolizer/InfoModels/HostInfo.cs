using Perfolizer.Phd.Base;

namespace Perfolizer.InfoModels;

public class HostInfo : AbstractInfo
{
    public OsInfo? Os { get; set; }
    public CpuInfo? Cpu { get; set; }
}