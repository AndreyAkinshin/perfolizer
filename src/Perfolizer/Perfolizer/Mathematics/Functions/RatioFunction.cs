using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public class RatioFunction : QuantileCompareFunction
    {
        public static readonly QuantileCompareFunction Instance = new RatioFunction();

        public RatioFunction([CanBeNull] IQuantileEstimator quantileEstimator = null) : base(quantileEstimator)
        {
        }

        protected override double CalculateValue(double quantileA, double quantileB) => quantileB / quantileA;
    }
}