using Perfolizer.Common;

namespace Perfolizer.Mathematics.GenericEstimators;

public interface IShiftEstimator
{
    double Shift(Sample x, Sample y);
}