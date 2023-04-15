using Perfolizer.Common;

namespace Perfolizer.Mathematics.ScaleEstimators;

public interface IScaleEstimator
{
    double Scale(Sample sample);
}