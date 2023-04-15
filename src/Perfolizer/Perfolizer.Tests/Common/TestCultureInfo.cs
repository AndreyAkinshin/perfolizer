using System.Globalization;
using Perfolizer.Common;

namespace Perfolizer.Tests.Common;

internal static class TestCultureInfo
{
    public static readonly CultureInfo Instance;

    static TestCultureInfo()
    {
        Instance = (CultureInfo) DefaultCultureInfo.Instance.Clone();
    }
}