using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.DispersionEstimators
{
    public class InvariantMedianAbsoluteDeviationEstimator : MedianAbsoluteDeviationEstimatorBase
    {
        public static readonly IMedianAbsoluteDeviationEstimator Instance = new InvariantMedianAbsoluteDeviationEstimator();
        
        protected override double GetScaleFactor(Sample sample) => 1;
        public override IQuantileEstimator QuantileEstimator => SimpleQuantileEstimator.Instance;
    }
}