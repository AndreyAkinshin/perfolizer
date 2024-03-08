using System.Globalization;
using Perfolizer.Common;

namespace Perfolizer.Tests.Infra;

public static class TestCultureInfo
{
    public static readonly CultureInfo Instance;

    static TestCultureInfo()
    {
        Instance = (CultureInfo)DefaultCultureInfo.Instance.Clone();
    }
}