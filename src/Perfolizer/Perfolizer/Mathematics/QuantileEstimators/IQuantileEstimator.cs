using JetBrains.Annotations;
using Perfolizer.Collections;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface IQuantileEstimator
    {
        double GetQuantile([NotNull] ISortedReadOnlyList<double> data, double probability);
    }
}