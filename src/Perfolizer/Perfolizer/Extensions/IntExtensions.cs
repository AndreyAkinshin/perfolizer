using Pragmastat.Metrology;

namespace Perfolizer.Extensions;

internal static class IntExtensions
{
    public static Measurement AsMeasurement(this int value) => new(value);
    public static int Abs(this int value) => Math.Abs(value);
}