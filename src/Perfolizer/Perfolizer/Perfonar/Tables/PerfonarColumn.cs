using System.Diagnostics;

namespace Perfolizer.Perfonar.Tables;

[DebuggerDisplay("{Selector}")]
public class PerfonarColumn(string title, string selector, PerfonarColumnDefinition definition)
{
    public string Title { get; } = title;
    public string Selector { get; } = selector;
    public PerfonarColumnDefinition Definition { get; } = definition;
}