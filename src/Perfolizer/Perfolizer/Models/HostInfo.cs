namespace Perfolizer.Models;

public class HostInfo : AbstractInfo
{
    public OsInfo? Os { get; set; }
    public CpuInfo? Cpu { get; set; }
}