using System.Diagnostics;
using Perfolizer.Helpers;

namespace Perfolizer.Phd.Functions;

[DebuggerDisplay("{Id}")]
public class PhdFunction(string id, string title, Func<Sample, object?> apply)
{
    public string Id { get; } = id;
    public string Title { get; } = title;
    public Func<Sample, object?> Apply { get; } = apply;
    public string? Legend { get; set; }

    public virtual Type ReturnType { get; } = typeof(object);
}

[DebuggerDisplay("{Id}")]
public class PhdFunction<T>(string id, string title, Func<Sample, T> apply) : PhdFunction(id, title, s => apply(s))
{
    public PhdFunction(string id, Func<Sample, T> apply) : this(id, StringHelper.FirstUpper(id), apply) { }
    public new Func<Sample, T> Apply { get; } = apply;
    public override Type ReturnType { get; } = typeof(T);
}