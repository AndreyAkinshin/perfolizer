using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public class RatioFunction : QuantileCompareFunction
    {
        public static readonly QuantileCompareFunction Instance = new RatioFunction();

        public RatioFunction(IQuantileEstimator? quantileEstimator = null) : base(quantileEstimator)
        {
        }

        protected override double CalculateValue(double quantileA, double quantileB) => quantileB / quantileA;
    }
}