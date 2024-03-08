using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Tables;

public class PhdAttributeColumn(string title, string selector, PhdColumnDefinition definition, PhdKey key)
    : PhdColumn(title, selector, definition)
{
    public PhdKey Key { get; } = key;
}