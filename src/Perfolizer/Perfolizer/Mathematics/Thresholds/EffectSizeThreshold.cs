using Perfolizer.Common;
using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Mathematics.Thresholds;

public class EffectSizeThreshold(double effectSize) : Threshold
{
    public static readonly AbsoluteThreshold Zero = new(0);
    public static EffectSizeThreshold Of(double effectSize) => new(effectSize);

    public override string ToString() => "ES=" + effectSize.ToStringInvariant("0.##");

    public override Sample Apply(Sample sample)
    {
        double scale = ShamosEstimator.Instance.Scale(sample);
        double shift = scale * effectSize;
        return Apply(sample, x => x + shift);
    }
}