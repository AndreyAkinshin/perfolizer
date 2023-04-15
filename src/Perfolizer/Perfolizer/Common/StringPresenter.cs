namespace Perfolizer.Common;

internal static class StringPresenter
{
    public static string Present(this IEnumerable<double> values, string format = "N2")
    {
        return "[" + string.Join("; ", values.Select(x => x.ToString(format))) + "]";
    }
}