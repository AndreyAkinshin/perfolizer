using System.Diagnostics;
using System.Text;
using Perfolizer.Collections;
using Perfolizer.InfoModels;
using Perfolizer.Metrology;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Tables;

[DebuggerDisplay("{Measurements.Count} measurements, {cells.Count} cells")]
public class PhdRow(EntryInfo entry)
{
    public EntryInfo Entry { get; } = entry;
    private readonly Dictionary<string, PhdCell> cells = new (StringComparer.OrdinalIgnoreCase);
    public List<Measurement> Measurements { get; } = [];

    public ICollection<PhdCell> Cells => cells.Values;

    public PhdCell this[PhdColumn column]
    {
        get => cells[column.Selector];
        set => cells[column.Selector] = value;
    }

    public PhdCell this[string selector]
    {
        get => cells[selector];
        set => cells[selector] = value;
    }

    public string BuildAttributeId()
    {
        var builder = new StringBuilder();
        foreach (var cell in cells.Values)
            if (cell.Column is PhdAttributeColumn)
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