using JetBrains.Annotations;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Dto;

[PublicAPI]
public class PhdEngine : PhdObject
{
    public string Name { get; set; } = "";
    public string? Version { get; set; }

    public PhdEngine SetDisplay()
    {
        Display = Version == null ? Name : $"{Name} v{Version}";
        return this;
    }

    public string ToBrandTitle() => Display = Version == null ? Name : $"{Name} v{Version}";
    public override string ToString() => ToBrandTitle();
}