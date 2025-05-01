using Perfolizer.Collections;
using Perfolizer.InfoModels;

namespace Perfolizer.Phd.Base;

public class PhdIndexedEntry(PhdKey key, EntryInfo entry, Dictionary<string, PhdProperty> properties)
{
    public PhdKey Key { get; } = key;
    public EntryInfo Entry { get; } = entry;
    public IReadOnlyCollection<PhdProperty> AllProperties => properties.Values;

    private PhdProperty? this[string key]
    {
        get => properties.GetValueOrDefault(key);
        set
        {
            if (value != null) properties[key] = value;
        }
    }

    public PhdProperty? this[PhdKey key]
    {
        get => this[key.Path];
        set => this[key.Path] = value;
    }
}