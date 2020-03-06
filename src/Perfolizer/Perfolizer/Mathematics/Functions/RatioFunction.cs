using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public class RatioFunction : DistributionCompareFunction
    {
        public RatioFunction([NotNull] IQuantileEstimator quantileEstimator) : base(quantileEstimator)
        {
        }

        public RatioFunction()
        {
        }

        protected override double[] CalculateValues(double[] quantilesA, double[] quantilesB)
        {
            int n = quantilesA.Length;
            var ratio = new double[n];
            for (int i = 0; i < n; i++)
                ratio[i] = quantilesB[i] / quantilesA[i];
            return ratio;
        }
    }
}