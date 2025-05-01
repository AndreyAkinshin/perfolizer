using System.Collections;
using System.Text;
using Perfolizer.Extensions;
using Perfolizer.InfoModels;
using Perfolizer.Metrology;

namespace Perfolizer.Perfonar.Base;

public class PerfonarIndex
{
    private readonly EntryInfo rootEntry;
    private readonly Dictionary<EntryInfo, PerfonarIndexedEntry> entries = new ();

    public IReadOnlyList<PerfonarKey> Keys { get; }
    public IReadOnlyCollection<EntryInfo> Entries => entries.Keys;
    public PerfonarIndexedEntry this[EntryInfo entry] => entries[entry];

    public PerfonarIndex(EntryInfo rootEntry)
    {
        this.rootEntry = rootEntry;
        IndexEntry(PerfonarKey.Empty, rootEntry, null);
        Keys = entries.Values
            .SelectMany(indexedEntry => indexedEntry
                .AllProperties
                .Select(property => property.Key)
                .Concat([indexedEntry.Key]))
            .Distinct()
            .Where(key => key.Path.IsNotBlank())
            .ToList();
    }

    private void IndexEntry(PerfonarKey key, EntryInfo entry, EntryInfo? parent)
    {
        IndexSelf(key, entry, parent);
        foreach (var nested in entry.Nested)
            IndexEntry(key, nested, entry);
    }

    private void IndexSelf(PerfonarKey key, EntryInfo entry, EntryInfo? parent)
    {
        var indexedEntry = entries[entry] = new PerfonarIndexedEntry(key, entry, new Dictionary<string, PerfonarProperty>());

        if (parent != null)
        {
            var indexedParent = entries[parent];
            foreach (var property in indexedParent.AllProperties)
                indexedEntry[property.Key] = property;
        }

        IndexAttributes(key, entry, entry);
    }

    private void IndexAttributes(PerfonarKey key, EntryInfo entry, object obj)
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

                var perfonarSubKey = key.Append(subKey, value.GetType());
                indexedEntry[perfonarSubKey] = new PerfonarProperty(perfonarSubKey, value);
            }
        }
        else
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                if (obj is EntryInfo && property.Name == nameof(EntryInfo.Nested))
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
                if (value is PerfonarMeta)
                    continue;
                var propertyKey = key.Append(property.Name, value.GetType());
                indexedEntry[propertyKey] = new PerfonarProperty(propertyKey, value);
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