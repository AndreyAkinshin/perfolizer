using System.Diagnostics;
using Perfolizer.InfoModels;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Tables;

[DebuggerDisplay("{Selector}/{Direction}")]
public class PhdSortPolicy(string selector, PhdSortDirection direction) : AbstractInfo
{
    public string Selector { get; } = selector;
    public PhdSortDirection Direction { get; } = direction;
}