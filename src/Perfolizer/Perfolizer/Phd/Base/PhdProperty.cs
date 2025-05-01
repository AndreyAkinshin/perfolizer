using System.Diagnostics;
using Perfolizer.InfoModels;

namespace Perfolizer.Phd.Base;

[DebuggerDisplay("{Key}={Value}")]
public class PhdProperty(PhdKey key, object value)
{
    public PhdKey Key { get; } = key;
    public object Value { get; } = value;
    public string Display => (Value is AbstractInfo phdObject ? phdObject.GetDisplay() : Value.ToString()) ?? "";
}