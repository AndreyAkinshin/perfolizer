using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.DispersionEstimators
{
    public interface IMedianAbsoluteDeviationEstimator
    {
        [NotNull] IQuantileEstimator QuantileEstimator { get; }
        
        double Calc([NotNull] Sample sample);
        double CalcLower([NotNull] Sample sample);
        double CalcUpper([NotNull] Sample sample);
    }
}