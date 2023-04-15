using Perfolizer.Common;

namespace Perfolizer.Mathematics.GenericEstimators
{
    public interface IMedianEstimator
    {
        double Median(Sample sample);
    }
}