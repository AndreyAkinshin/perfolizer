using Perfolizer.Helpers;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Dto;

public class PhdOs : PhdObject
{
    /// <summary>
    /// E.g., Windows, Linux, macOS
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// E.g., Ubuntu, Fedora, CentOS, Debian, RHEL
    /// </summary>
    public string? Distro { get; set; }

    public string? Version { get; set; }

    public string? KernelVersion { get; set; }

    /// <summary>
    /// E.g., Docker
    /// </summary>
    public string? Container { get; set; }

    public override string GetDisplay() => Display ?? this.ToBrandString();
    public override string ToString() => this.ToBrandString();
}