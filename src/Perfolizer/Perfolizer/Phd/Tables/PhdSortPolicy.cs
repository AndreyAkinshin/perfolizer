using System.Diagnostics;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Tables;

[DebuggerDisplay("{Selector}/{Direction}")]
public class PhdSortPolicy(string selector, PhdSortDirection direction) : PhdObject
{
    public string Selector { get; } = selector;
    public PhdSortDirection Direction { get; } = direction;
}