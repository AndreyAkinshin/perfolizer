using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public class ShiftFunction : DistributionCompareFunction
    {
        public ShiftFunction([NotNull] IQuantileEstimator quantileEstimator) : base(quantileEstimator)
        {
        }

        public ShiftFunction()
        {
        }

        protected override double[] CalculateValues(double[] quantilesA, double[] quantilesB)
        {
            int n = quantilesA.Length;
            var shift = new double[n];
            for (int i = 0; i < n; i++)
                shift[i] = quantilesB[i] - quantilesA[i];
            return shift;
        }
    }
}