using System.Diagnostics;

namespace Perfolizer.Perfonar.Tables;

[DebuggerDisplay("{Column.Title}={Value}")]
public class PerfonarCell(PerfonarColumn column, object? value)
{
    public PerfonarColumn Column { get; } = column;
    public object? Value { get; } = value;
}