using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.RangeEstimators
{
    public class ShiftRangeEstimator : DistributionCompareRangeEstimator
    {
        public static readonly DistributionCompareRangeEstimator Instance = new ShiftRangeEstimator();
        
        public ShiftRangeEstimator(IQuantileEstimator quantileEstimator) : base(new ShiftFunction(quantileEstimator))
        {
        }

        public ShiftRangeEstimator() : base(new ShiftFunction())
        {
        }
    }
}