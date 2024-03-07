using Perfolizer.Common;

namespace Perfolizer.Mathematics.EffectSizes;

public interface IEffectSizeEstimator
{
    double EffectSize(Sample x, Sample y);
}