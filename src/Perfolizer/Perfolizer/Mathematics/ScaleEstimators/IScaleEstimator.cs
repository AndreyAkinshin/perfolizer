using Pragmastat;

namespace Perfolizer.Mathematics.ScaleEstimators;

[Obsolete("Use Pragmastat.Estimators.SpreadEstimator instead.")]
public interface IScaleEstimator
{
    double Scale(Sample sample);
}