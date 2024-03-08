using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.EffectSizes;

public class CohenDEffectSize : IEffectSizeEstimator
{
    public static readonly CohenDEffectSize Instance = new();

    public double EffectSize(Sample x, Sample y)
    {
        Assertion.NotNull(nameof(x), x);
        Assertion.NotNull(nameof(y), y);
        if (x.Size < 2)
            throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} should contain at least 2 elements");
        if (y.Size < 2)
            throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} should contain at least 2 elements");

        int nx = x.Size;
        int ny = y.Size;
        var mx = Moments.Create(x);
        var my = Moments.Create(y);
        double s = Sqrt(((nx - 1) * mx.Variance + (ny - 1) * my.Variance) / (nx + ny - 2));
        return (my.Mean - mx.Mean) / s;
    }
}