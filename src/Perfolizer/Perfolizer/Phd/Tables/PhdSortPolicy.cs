using System.Diagnostics;

namespace Perfolizer.Phd.Tables;

[DebuggerDisplay("{Selector}/{Direction}")]
public class PhdSortPolicy(string selector, PhdSortDirection direction)
{
    public string Selector { get; } = selector;
    public PhdSortDirection Direction { get; } = direction;
}