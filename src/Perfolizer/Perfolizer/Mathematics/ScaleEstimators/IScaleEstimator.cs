using Pragmastat;

namespace Perfolizer.Mathematics.ScaleEstimators;

public interface IScaleEstimator
{
    double Scale(Sample sample);
}