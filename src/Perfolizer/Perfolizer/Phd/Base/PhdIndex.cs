using System.Text;
using Perfolizer.Extensions;

namespace Perfolizer.Phd.Base;

public class PhdIndex
{
    private readonly PhdEntry rootEntry;
    private readonly Dictionary<PhdEntry, PhdIndexedEntry> entries = new ();
    public IReadOnlyList<PhdKey> Keys { get; }
    public IReadOnlyCollection<PhdEntry> Entries => entries.Keys;
    public PhdIndexedEntry this[PhdEntry entry] => entries[entry];

    public PhdIndex(PhdEntry rootEntry)
    {
        this.rootEntry = rootEntry;
        IndexEntry(PhdKey.Empty, rootEntry, null);
        Keys = entries.Values
            .SelectMany(indexedEntry => indexedEntry
                .AllProperties
                .Select(property => property.Key)
                .Concat([indexedEntry.Key]))
            .Distinct()
            .Where(key => key.Path.IsNotBlank())
            .ToList();
    }

    private void IndexEntry(PhdKey key, PhdEntry entry, PhdEntry? parent)
    {
        IndexSelf(key, entry, parent);
        foreach (var child in entry.Children)
            IndexEntry(key, child, entry);
    }

    private void IndexSelf(PhdKey key, PhdEntry entry, PhdEntry? parent)
    {
        var indexedEntry = entries[entry] = new PhdIndexedEntry(key, entry, new Dictionary<string, PhdProperty>());

        if (parent != null)
        {
            var indexedParent = entries[parent];
            foreach (var property in indexedParent.AllProperties)
                indexedEntry[property.Key] = property;
        }

        IndexAttributes(key, entry, entry.Attributes);
    }

    private void IndexAttributes(PhdKey key, PhdEntry entry, object obj)
    {
        var indexedEntry = entries[entry];
        foreach (var property in obj.GetType().GetProperties())
        {
            object? value;
            try
            {
                value = property.GetValue(obj);
            }
            catch (Exception)
            {
                continue;
            }
            if (value == null || value.ToString().IsBlank())
                continue;
            var propertyKey = key.Append(property.Name, value.GetType());
            indexedEntry[propertyKey] = new PhdProperty(propertyKey, value);
            if (!value.GetType().IsPrimitive && value is not string)
                IndexAttributes(propertyKey, entry, value);
        }
    }

    public string Dump()
    {
        var builder = new StringBuilder();
        builder.AppendLine("[Keys]");
        foreach (var key in Keys.OrderBy(key => key.Path, StringComparer.OrdinalIgnoreCase))
            builder.AppendLine($"  {key.Path}");
        builder.AppendLine();

        int index = 0;
        foreach (var entry in rootEntry.Traverse())
        {
            builder.AppendLine($"[Entry{index++}]");
            var indexedEntry = entries[entry];
            foreach (var property in indexedEntry.AllProperties.OrderBy(property => property.Key.Path))
                builder.AppendLine($"  {property.Key.Path} = {property.Value}");
        }

        return builder.ToString().Trim();
    }
}