using JetBrains.Annotations;

namespace Perfolizer.Models;

[PublicAPI]
public class EngineInfo : AbstractInfo
{
    public string Name { get; set; } = "";
    public string? Version { get; set; }

    public override string GetDisplay() => Display ?? ToBrandTitle();
    public string ToBrandTitle() => Version == null ? Name : $"{Name} v{Version}";
    public override string ToString() => ToBrandTitle();
}