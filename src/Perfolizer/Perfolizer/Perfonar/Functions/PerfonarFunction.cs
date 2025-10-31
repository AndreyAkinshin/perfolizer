using System.Diagnostics;
using Perfolizer.Helpers;
using Pragmastat;

namespace Perfolizer.Perfonar.Functions;

[DebuggerDisplay("{Id}")]
public class PerfonarFunction(string id, string title, Func<Sample, object?> apply)
{
    public string Id { get; } = id;
    public string Title { get; } = title;
    public Func<Sample, object?> Apply { get; } = apply;
    public string? Legend { get; set; }

    public virtual Type ReturnType { get; } = typeof(object);
}

[DebuggerDisplay("{Id}")]
public class PerfonarFunction<T>(string id, string title, Func<Sample, T> apply) : PerfonarFunction(id, title, s => apply(s))
{
    public PerfonarFunction(string id, Func<Sample, T> apply) : this(id, StringHelper.FirstUpper(id), apply) { }
    public new Func<Sample, T> Apply { get; } = apply;
    public override Type ReturnType { get; } = typeof(T);
}