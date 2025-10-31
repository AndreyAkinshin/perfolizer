using System.Diagnostics;
using System.Text;
using Perfolizer.Collections;
using Perfolizer.Models;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Perfonar.Tables;

[DebuggerDisplay("{Measurements.Count} measurements, {cells.Count} cells")]
public class PerfonarRow(EntryInfo entry)
{
    public EntryInfo Entry { get; } = entry;
    private readonly Dictionary<string, PerfonarCell> cells = new (StringComparer.OrdinalIgnoreCase);
    public List<Measurement> Measurements { get; } = [];

    public ICollection<PerfonarCell> Cells => cells.Values;

    public PerfonarCell this[PerfonarColumn column]
    {
        get => cells[column.Selector];
        set => cells[column.Selector] = value;
    }

    public PerfonarCell this[string selector]
    {
        get => cells[selector];
        set => cells[selector] = value;
    }

    public string BuildAttributeId()
    {
        var builder = new StringBuilder();
        foreach (var cell in cells.Values)
            if (cell.Column is PerfonarAttributeColumn)
            {
                if (builder.Length > 0)
                    builder.Append('_');
                if (cell.Value is not Enum)
                    builder.Append(cell.Column.Title);
                builder.Append(cell.Value);
            }
        return builder.ToString();
    }

    public Sample ToSample() => Measurements.Select(m => m.NominalValue).ToSample(Measurements.First().Unit);
}