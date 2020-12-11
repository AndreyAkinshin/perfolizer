namespace Perfolizer.Common
{
    internal static class StringExtensions
    {
        public static string ToStringInvariant(this double value) => value.ToString(DefaultCultureInfo.Instance);
        public static string ToStringInvariant(this double value, string format) => value.ToString(format, DefaultCultureInfo.Instance);
    }
}