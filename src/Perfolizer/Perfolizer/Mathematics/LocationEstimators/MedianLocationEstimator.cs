using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.LocationEstimators;

public class MedianLocationEstimator(IQuantileEstimator quantileEstimator) : ILocationEstimator
{
    public double Location(Sample x) => quantileEstimator.Median(x);
}