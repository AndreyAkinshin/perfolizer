using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface IQuantileEstimator
    {
        /// <summary>
        /// Calculates the requested quantile estimation based on the given sample
        /// </summary>
        /// <param name="sample">A sample</param>
        /// <param name="probability">Value in range [0;1] that describes the requested quantile</param>
        /// <returns>Quantile estimation for the given sample</returns>
        double GetQuantile([NotNull] Sample sample, Probability probability);
        
        bool SupportsWeightedSamples { get; }
        
        string Alias { get; }
    }
}