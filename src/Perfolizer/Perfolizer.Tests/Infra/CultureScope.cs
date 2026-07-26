using System.Globalization;

namespace Perfolizer.Tests.Infra;

/// <summary>
/// Runs an assertion under a specific ambient culture and restores the previous one afterwards.
/// Perfolizer output is expected to be culture-independent, and the test host usually runs under
/// an invariant-like culture, so such regressions are invisible without an explicit switch.
/// </summary>
public static class CultureScope
{
    public static void Run(string cultureName, Action action)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }
}
