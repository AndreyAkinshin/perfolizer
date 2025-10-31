using Perfolizer.Mathematics.Common;
using Pragmastat;

namespace Perfolizer.Mathematics.QuantileEstimators;

public interface IQuantileConfidenceIntervalEstimator
{
    public ConfidenceIntervalEstimator QuantileConfidenceIntervalEstimator(Sample sample, Probability probability);
}