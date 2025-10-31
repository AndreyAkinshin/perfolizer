using Perfolizer.Mathematics.QuantileEstimators;
using Pragmastat;
using Pragmastat.Estimators;
using Pragmastat.Metrology;

namespace Perfolizer.Mathematics.LocationEstimators;

public class MedianLocationEstimator(IQuantileEstimator quantileEstimator) : IOneSampleEstimator
{
    public Measurement Estimate(Sample x) => quantileEstimator.Median(x);
}