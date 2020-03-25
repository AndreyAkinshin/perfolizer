using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.RangeEstimators
{
    public class RatioRangeEstimator : DistributionCompareRangeEstimator
    {
        public static readonly DistributionCompareRangeEstimator Instance = new RatioRangeEstimator();
        
        public RatioRangeEstimator(IQuantileEstimator quantileEstimator) : base(new RatioFunction(quantileEstimator))
        {
        }

        public RatioRangeEstimator() : base(new RatioFunction())
        {
        }
    }
}