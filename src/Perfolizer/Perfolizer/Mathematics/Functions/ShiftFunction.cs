using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public class ShiftFunction : DistributionCompareFunction
    {
        public static readonly DistributionCompareFunction Instance = new ShiftFunction();

        public ShiftFunction([CanBeNull] IQuantileEstimator quantileEstimator = null) : base(quantileEstimator)
        {
        }

        protected override double CalculateValue(double quantileA, double quantileB) => quantileB - quantileA;
    }
}