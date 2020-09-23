using JetBrains.Annotations;
using Perfolizer.Collections;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface IQuantileEstimator
    {
        /// <summary>
        /// Calculates the requested quantile estimation based on the given sample
        /// </summary>
        /// <param name="data">A sample</param>
        /// <param name="probability">Value in range [0;1] that describes the requested quantile</param>
        /// <returns>Quantile estimation for the given sample</returns>
        double GetQuantile([NotNull] ISortedReadOnlyList<double> data, double probability);
    }
}