using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.DispersionEstimators
{
    public interface IMedianAbsoluteDeviationEstimator
    {
        IQuantileEstimator QuantileEstimator { get; }
        
        double Calc(Sample sample);
        double CalcLower(Sample sample);
        double CalcUpper(Sample sample);
    }
}