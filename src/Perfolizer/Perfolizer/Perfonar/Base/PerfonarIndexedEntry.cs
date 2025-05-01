using Perfolizer.Collections;
using Perfolizer.Models;

namespace Perfolizer.Perfonar.Base;

public class PerfonarIndexedEntry(PerfonarKey key, EntryInfo entry, Dictionary<string, PerfonarProperty> properties)
{
    public PerfonarKey Key { get; } = key;
    public EntryInfo Entry { get; } = entry;
    public IReadOnlyCollection<PerfonarProperty> AllProperties => properties.Values;

    private PerfonarProperty? this[string key]
    {
        get => properties.GetValueOrDefault(key);
        set
        {
            if (value != null) properties[key] = value;
        }
    }

    public PerfonarProperty? this[PerfonarKey key]
    {
        get => this[key.Path];
        set => this[key.Path] = value;
    }
}