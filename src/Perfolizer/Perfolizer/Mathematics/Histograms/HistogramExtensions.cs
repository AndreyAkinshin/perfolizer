using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Horology;

namespace Perfolizer.Mathematics.Histograms;

[PublicAPI]
public static class HistogramExtensions
{
    [PublicAPI, Pure]
    public static int GetBinCount(this Histogram histogram) => histogram.Bins.Length;

    [PublicAPI, Pure]
    public static IEnumerable<double> AllValues(this Histogram histogram) => histogram.Bins.SelectMany(bin => bin.Values);
        
    public static Func<double, string> CreateNanosecondFormatter(this Histogram histogram, CultureInfo? cultureInfo = null, string format = "0.000")
    {
        var timeUnit = TimeUnit.GetBestTimeUnit(histogram.Bins.SelectMany(bin => bin.Values).ToArray());
        return value => TimeInterval.FromNanoseconds(value).ToString(timeUnit, format, cultureInfo);
    }
}