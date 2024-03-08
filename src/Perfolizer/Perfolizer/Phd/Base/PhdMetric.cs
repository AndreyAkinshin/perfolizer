using Microsoft.VisualBasic;

namespace Perfolizer.Phd.Base;

public class PhdMetric(string? id, int? version) : PhdObject
{
    public static readonly PhdMetric Empty = new (null, null);

    public string? Id { get; set; } = id;
    public int? Version { get; set; } = version;

    public override string ToString() => Version is null or 0
        ? Id ?? ""
        : $"{Id} v{Version}".Trim();

    public static implicit operator PhdMetric(string id) => new (id, null);
    public bool IsEmpty => Id == null && Version == null;
}