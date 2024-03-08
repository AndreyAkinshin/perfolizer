using System.Diagnostics;

namespace Perfolizer.Phd.Base;

[DebuggerDisplay("{Key}={Value}")]
public class PhdProperty(PhdKey key, object value)
{
    public PhdKey Key { get; } = key;
    public object Value { get; } = value;
    public string Display => (Value is PhdObject phdObject ? phdObject.GetDisplay() : Value.ToString()) ?? "";
}