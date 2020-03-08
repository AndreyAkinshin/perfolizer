using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface IQuantileEstimator
    {
        double GetQuantileFromSorted([NotNull] IReadOnlyList<double> data, double quantile);
    }
}