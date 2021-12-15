using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.DispersionEstimators
{
    public class NonScaledMedianAbsoluteDeviationEstimator : MedianAbsoluteDeviationEstimatorBase
    {
        public static IMedianAbsoluteDeviationEstimator Instance = new NonScaledMedianAbsoluteDeviationEstimator();
        
        protected override double GetScaleFactor(Sample sample) => 1;
        public override IQuantileEstimator QuantileEstimator => SimpleQuantileEstimator.Instance;
    }
}