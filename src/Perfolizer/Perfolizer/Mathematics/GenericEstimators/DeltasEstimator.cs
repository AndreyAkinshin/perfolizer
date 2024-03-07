using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.EffectSizes;

namespace Perfolizer.Mathematics.GenericEstimators;

public class DeltasEstimator(
    IShiftEstimator shiftEstimator
    , IRatioEstimator ratioEstimator
    , IEffectSizeEstimator effectSizeEstimator)
{
    [PublicAPI]
    public static readonly DeltasEstimator HodgesLehmannShamos = new(
        HodgesLehmannEstimator.Instance,
        HodgesLehmannEstimator.Instance,
        DiffEffectSize.HodgesLehmannShamos
    );

    [PublicAPI]
    public Deltas Deltas(Sample x, Sample y) => new(
        shiftEstimator.Shift(x, y),
        ratioEstimator.Ratio(x, y),
        effectSizeEstimator.EffectSize(x, y)
    );
}