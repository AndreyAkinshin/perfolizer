using Perfolizer.Common;

namespace Perfolizer.Mathematics.GenericEstimators;

public interface IRatioEstimator
{
    double Ratio(Sample x, Sample y);
}