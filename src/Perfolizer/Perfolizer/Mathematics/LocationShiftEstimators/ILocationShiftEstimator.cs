using Perfolizer.Common;

namespace Perfolizer.Mathematics.LocationShiftEstimators
{
    public interface ILocationShiftEstimator
    {
        double LocationShift(Sample a, Sample b);
    }
}