using Perfolizer.Metrology;

namespace Perfolizer.Extensions;

internal static class IntExtensions
{
    public static Measurement AsMeasurement(this int value) => new (value, NumberUnit.Instance);
    public static int Abs(this int value) => Math.Abs(value);
}