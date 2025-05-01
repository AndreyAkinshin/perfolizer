namespace Perfolizer.Perfonar.Tables;

/// <summary>
/// A named collection of shared cells
/// </summary>
public class PerfonarCloud(string id, IReadOnlyList<PerfonarCell> cells)
{
    public string Id { get; } = id;
    public IReadOnlyList<PerfonarCell> Cells { get; } = cells;
}