namespace Perfolizer.Phd.Tables;

/// <summary>
/// A named collection of shared cells
/// </summary>
public class PhdCloud(string id, IReadOnlyList<PhdCell> cells)
{
    public string Id { get; } = id;
    public IReadOnlyList<PhdCell> Cells { get; } = cells;
}