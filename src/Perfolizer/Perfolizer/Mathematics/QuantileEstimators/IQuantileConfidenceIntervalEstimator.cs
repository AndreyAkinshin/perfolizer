using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface IQuantileConfidenceIntervalEstimator
    {
        public ConfidenceIntervalEstimator QuantileConfidenceIntervalEstimator(Sample sample, Probability probability);
    }
}