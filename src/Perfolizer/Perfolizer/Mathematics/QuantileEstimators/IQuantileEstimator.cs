using JetBrains.Annotations;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface IQuantileEstimator
    {
        [NotNull] double[] GetQuantiles([NotNull] double[] data, [NotNull] double[] quantiles);
    }
}