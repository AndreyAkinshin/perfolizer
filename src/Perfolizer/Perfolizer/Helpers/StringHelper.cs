using Perfolizer.Extensions;

namespace Perfolizer.Helpers;

public static class StringHelper
{
    public static string FirstUpper(string s) => s.IsBlank() ? s : char.ToUpper(s[0]) + s.Substring(1);
}