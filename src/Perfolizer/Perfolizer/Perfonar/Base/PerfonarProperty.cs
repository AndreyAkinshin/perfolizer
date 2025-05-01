using System.Diagnostics;
using Perfolizer.Models;

namespace Perfolizer.Perfonar.Base;

[DebuggerDisplay("{Key}={Value}")]
public class PerfonarProperty(PerfonarKey key, object value)
{
    public PerfonarKey Key { get; } = key;
    public object Value { get; } = value;
    public string Display => (Value is AbstractInfo perfonarObject ? perfonarObject.GetDisplay() : Value.ToString()) ?? "";
}