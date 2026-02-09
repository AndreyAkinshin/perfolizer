using System.Globalization;

namespace Perfolizer.Common;

internal static class DefaultCultureInfo
{
    public static readonly CultureInfo Instance;

    static DefaultCultureInfo()
    {
        Instance = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        Instance.NumberFormat.NumberDecimalSeparator = ".";
        Instance.NumberFormat.NumberGroupSeparator = "";
    }
}