namespace Perfolizer.Extensions;

internal static class StringExtensions
{
    public static bool IsBlank(this string? value) => string.IsNullOrWhiteSpace(value);
    public static bool IsNotBlank(this string? value) => !value.IsBlank();
    public static bool EquationsIgnoreCase(this string a, string b) => a.Equals(b, StringComparison.OrdinalIgnoreCase);
}