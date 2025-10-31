using System.Globalization;
using Perfolizer.Common;
using Pragmastat;

namespace Perfolizer.Extensions;

internal static class DoubleExtensions
{
    public static string ToStringInvariant(this double value) => value.ToString(DefaultCultureInfo.Instance);
    public static string ToStringInvariant(this double value, string format) => value.ToString(format, DefaultCultureInfo.Instance);

    public static string ToStringInvariant(this Probability p) => p.ToString(DefaultCultureInfo.Instance);
    public static string ToStringInvariant(this Probability p, string format) => p.ToString(format, DefaultCultureInfo.Instance);
    
    public static string Format(this double value, string? format = null, IFormatProvider? formatProvider = null)
    {
        return value.ToString(format ?? "G", formatProvider ?? CultureInfo.InvariantCulture);
    }
}