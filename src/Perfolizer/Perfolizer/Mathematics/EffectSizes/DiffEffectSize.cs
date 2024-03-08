using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Mathematics.EffectSizes;

// TODO: support defensive scale, see https://aakinshin.net/posts/scale-measure-for-discrete-case/
public class DiffEffectSize(IShiftEstimator shiftEstimator, IScaleEstimator scaleEstimator)
    : IEffectSizeEstimator
{
    public static readonly DiffEffectSize HodgesLehmannShamos =
        new(HodgesLehmannEstimator.Instance, ShamosEstimator.Instance);

    public double EffectSize(Sample x, Sample y)
    {
        double shift = shiftEstimator.Shift(x, y);
        double s1 = scaleEstimator.Scale(x);
        double s2 = scaleEstimator.Scale(y);
        double s = GammaEffectSizeFunction.Pooled(x.Size, y.Size, s1, s2);
        return shift / s;
    }
}