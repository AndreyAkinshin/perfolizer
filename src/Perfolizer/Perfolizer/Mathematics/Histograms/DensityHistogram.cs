using System.Text;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Histograms;

public class DensityHistogram
{
    public IReadOnlyList<DensityHistogramBin> Bins { get; }

    public double GlobalLower => Bins.First().Lower;
    public double GlobalUpper => Bins.Last().Upper;

    public DensityHistogram(IReadOnlyList<DensityHistogramBin> bins)
    {
        Assertion.NotNullOrEmpty(nameof(bins), bins);
        Bins = bins;
    }

    public string Present(string format = "N2")
    {
        var builder = new StringBuilder();
        foreach (var bin in Bins)
            builder.AppendLine(string.Format(DefaultCultureInfo.Instance, "[{0};{1}]: {2}",
                bin.Lower.ToString(format),
                bin.Upper.ToString(format),
                bin.Height.ToString(format)));
        return builder.TrimEnd().ToString();
    }
}