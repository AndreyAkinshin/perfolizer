using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public class ShiftFunction : DistributionCompareFunction
    {
        public static readonly DistributionCompareFunction Instance = new ShiftFunction();
        
        public ShiftFunction([NotNull] IQuantileEstimator quantileEstimator) : base(quantileEstimator)
        {
        }

        public ShiftFunction()
        {
        }

        protected override double[] CalculateValues(double[] probabilitiesA, double[] probabilitiesB)
        {
            int n = probabilitiesA.Length;
            var shift = new double[n];
            for (int i = 0; i < n; i++)
                shift[i] = probabilitiesB[i] - probabilitiesA[i];
            return shift;
        }
    }
}