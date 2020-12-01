using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface IQuantileConfidenceIntervalEstimator
    {
        public ConfidenceIntervalEstimator GetQuantileConfidenceIntervalEstimator(Sample sample, double probability);
    }
}