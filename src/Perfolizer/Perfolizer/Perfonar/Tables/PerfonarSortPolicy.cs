using System.Diagnostics;
using Perfolizer.Models;

namespace Perfolizer.Perfonar.Tables;

[DebuggerDisplay("{Selector}/{Direction}")]
public class PerfonarSortPolicy(string selector, PerfonarSortDirection direction) : AbstractInfo
{
    public string Selector { get; } = selector;
    public PerfonarSortDirection Direction { get; } = direction;
}