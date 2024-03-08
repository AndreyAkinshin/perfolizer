using System.Diagnostics;

namespace Perfolizer.Phd.Tables;

[DebuggerDisplay("{Column.Title}={Value}")]
public class PhdCell(PhdColumn column, object? value)
{
    public PhdColumn Column { get; } = column;
    public object? Value { get; } = value;
}