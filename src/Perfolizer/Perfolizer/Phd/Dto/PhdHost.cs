using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Dto;

public class PhdHost : PhdObject
{
    public PhdOs? Os { get; set; }
    public PhdCpu? Cpu { get; set; }
}