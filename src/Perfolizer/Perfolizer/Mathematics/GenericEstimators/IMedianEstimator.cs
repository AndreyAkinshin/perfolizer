using Pragmastat;

namespace Perfolizer.Mathematics.GenericEstimators;

public interface IMedianEstimator
{
    double Median(Sample x);
}