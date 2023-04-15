using Perfolizer.Common;

namespace Perfolizer.Mathematics.GenericEstimators;

public interface ILocationShiftEstimator
{
    double LocationShift(Sample x, Sample y);
}