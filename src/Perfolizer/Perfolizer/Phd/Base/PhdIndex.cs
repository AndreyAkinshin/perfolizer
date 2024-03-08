using System.Collections;
using System.Text;
using Perfolizer.Extensions;
using Perfolizer.Metrology;

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
        foreach (var nested in entry.Nested)
            IndexEntry(key, nested, entry);
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

        IndexAttributes(key, entry, entry);
    }

    private void IndexAttributes(PhdKey key, PhdEntry entry, object obj)
    {
        var indexedEntry = entries[entry];
        if (obj is IDictionary dictionary)
        {
            foreach (DictionaryEntry dictionaryEntry in dictionary)
            {
                string? subKey = dictionaryEntry.Key.ToString();
                object? value = dictionaryEntry.Value;
                if (subKey == null || value == null || value.ToString().IsBlank())
                    continue;

                var phdSubKey = key.Append(subKey, value.GetType());
                indexedEntry[phdSubKey] = new PhdProperty(phdSubKey, value);
            }
        }
        else
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                if (obj is PhdEntry && property.Name == nameof(PhdEntry.Nested))
                    continue;
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
                if (value is PhdMeta)
                    continue;
                var propertyKey = key.Append(property.Name, value.GetType());
                indexedEntry[propertyKey] = new PhdProperty(propertyKey, value);
                if (!value.GetType().IsPrimitive &&
                    value is not string &&
                    value is not MeasurementUnit)
                    IndexAttributes(propertyKey, entry, value);
            }
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
            if (entry.Value == null) continue;

            builder.AppendLine($"[Entry{index++}]");
            var indexedEntry = entries[entry];
            foreach (var property in indexedEntry.AllProperties.OrderBy(property => property.Key.Path))
            {
                string value = property.Display;
                if (property.Value is IDictionary)
                    value = "<dictionary>";
                else if (value.IsBlank() && !property.Value.GetType().IsPrimitive)
                    value = "<object>";
                builder.AppendLine($"  {property.Key.Path} = {value}");
            }
        }

        return builder.ToString().Trim();
    }
}