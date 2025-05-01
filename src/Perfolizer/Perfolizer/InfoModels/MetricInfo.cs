namespace Perfolizer.InfoModels;

public class MetricInfo(string? id, int? version) : AbstractInfo
{
    public static readonly MetricInfo Empty = new (null, null);

    public string? Id { get; set; } = id;
    public int? Version { get; set; } = version;

    public override string ToString() => Version is null or 0
        ? Id ?? ""
        : $"{Id} v{Version}".Trim();

    public static implicit operator MetricInfo(string id) => new (id, null);
    public bool IsEmpty => Id == null && Version == null;
}