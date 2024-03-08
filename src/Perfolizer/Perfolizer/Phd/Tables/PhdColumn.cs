using System.Diagnostics;

namespace Perfolizer.Phd.Tables;

[DebuggerDisplay("{Selector}")]
public class PhdColumn(string title, string selector, PhdColumnDefinition definition)
{
    public string Title { get; } = title;
    public string Selector { get; } = selector;
    public PhdColumnDefinition Definition { get; } = definition;
}