namespace Perfolizer.Extensions;

internal static class StringExtensions
{
    public static bool IsBlank(this string? value) => string.IsNullOrWhiteSpace(value);
    public static bool IsNotBlank(this string? value) => !value.IsBlank();
    public static bool EquationsIgnoreCase(this string a, string b) => a.Equals(b, StringComparison.OrdinalIgnoreCase);
    public static bool StartWithIgnoreCase(this string a, string b) => a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
    public static string JoinToString(this IEnumerable<string> values, string separator) => string.Join(separator, values);
    public static string CapitalizeFirst(this string s) => s.Length == 0 ? s : char.ToUpper(s[0]) + s.Substring(1);
    public static string ToCamelCase(this string s) => s.Length == 0 ? s : char.ToLower(s[0]) + s.Substring(1);
    public static bool StartsWith(this string s, char c) => s.Length > 0 && s[0] == c;
}