using System.Text;
using Perfolizer.Extensions;

namespace Perfolizer.Helpers;

internal static class UnicodeHelper
{
    /// <summary>
    /// Greek letter μ (mu)
    /// </summary>
    public const char Mu = '\u03BC';

    private static readonly Dictionary<char, string> CharMap = new()
    {
        { Mu, "u" }
    };

    public static string ConvertToAscii(this string s)
    {
        if (s.IsBlank()) return s;

        var builder = new StringBuilder();
        foreach (char c in s)
            builder.Append(CharMap.TryGetValue(c, out string? replacement) ? replacement : c.ToString());
        return builder.ToString();
    }
}