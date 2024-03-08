using Perfolizer.Collections;
using Perfolizer.Phd.Dto;

namespace Perfolizer.Phd.Base;

public class PhdEntry(PhdAttributes attributes)
{
    public PhdEntry? Parent { get; private set; }

    public PhdAttributes Attributes { get; } = attributes;

    private readonly List<PhdMetric> metrics = [];
    public IReadOnlyList<PhdMetric> Metrics => metrics;

    private readonly List<PhdEntry> children = [];
    public IReadOnlyList<PhdEntry> Children => children;

    public PhdMeta? ResolveMeta()
    {
        if (Attributes.Meta != null)
            return Attributes.Meta;
        return Children.Select(child => child.ResolveMeta()).WhereNotNull().FirstOrDefault();
    }

    // Metrics
    public bool Remove(PhdMetric metric) => metrics.Remove(metric);

    public PhdEntry Add(params PhdMetric[] newMetrics)
    {
        metrics.AddRange(newMetrics);
        return this;
    }

    // Tree
    public PhdEntry Add(params PhdEntry[] entries)
    {
        foreach (var entry in entries)
        {
            children.Add(entry);
            entry.Parent = this;
        }
        return this;
    }

    public bool Remove(PhdEntry entry)
    {
        if (entry.Parent == this && children.Remove(entry))
        {
            entry.Parent = null;
            return true;
        }
        return false;
    }

    public IEnumerable<PhdEntry> Traverse()
    {
        yield return this;
        foreach (var entry in children.SelectMany(entry => entry.Traverse()))
            yield return entry;
    }

    // Serialization

    public PhdRawEntry ToRaw()
    {
        var root = new PhdRawEntry { Attributes = Attributes };
        foreach (var metric in metrics)
            root.Metrics.Add(metric.Serialize());
        foreach (var child in children)
            root.Children.Add(child.ToRaw());
        return root;
    }

    public static T Deserialize<T>(PhdRawEntry phdRawEntry, PhdEntry? parent = null) where T : PhdEntry
    {
        var perfEntry = (T)Activator.CreateInstance(typeof(T), phdRawEntry.Attributes);
        perfEntry.Parent = parent;
        foreach (var metric in phdRawEntry.Metrics)
            perfEntry.metrics.Add(PhdMetric.Deserialize(metric));
        foreach (var child in phdRawEntry.Children)
            perfEntry.children.Add(Deserialize<T>(child, perfEntry));
        return perfEntry;
    }
}