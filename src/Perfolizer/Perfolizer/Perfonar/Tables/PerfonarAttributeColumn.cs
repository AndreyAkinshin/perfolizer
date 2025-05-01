using Perfolizer.Perfonar.Base;

namespace Perfolizer.Perfonar.Tables;

public class PerfonarAttributeColumn(string title, string selector, PerfonarColumnDefinition definition, PerfonarKey key)
    : PerfonarColumn(title, selector, definition)
{
    public PerfonarKey Key { get; } = key;
}