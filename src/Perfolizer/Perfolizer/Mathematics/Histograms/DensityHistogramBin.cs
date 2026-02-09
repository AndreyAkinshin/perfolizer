using Perfolizer.Common;
using Perfolizer.Extensions;

namespace Perfolizer.Mathematics.Histograms;

public class DensityHistogramBin
{
    public double Lower { get; }
    public double Upper { get; }
    public double Height { get; }

    public double Middle => (Lower + Upper) / 2;

    public DensityHistogramBin(double lower, double upper, double height)
    {
        Assertion.NonNegative(nameof(height), height);
        Assertion.Positive("width", upper - lower);

        Lower = lower;
        Upper = upper;
        Height = height;
    }

    public override string ToString()
    {
        return $"[{Lower.ToStringInvariant()}; {Upper.ToStringInvariant()}] H = {Height.ToStringInvariant()}";
    }
}