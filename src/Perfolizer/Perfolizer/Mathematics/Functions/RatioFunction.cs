using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public class RatioFunction : DistributionCompareFunction
    {
        public static readonly DistributionCompareFunction Instance = new RatioFunction();
        
        public RatioFunction([NotNull] IQuantileEstimator quantileEstimator) : base(quantileEstimator)
        {
        }

        public RatioFunction()
        {
        }

        protected override double[] CalculateValues(double[] probabilitiesA, double[] probabilitiesB)
        {
            int n = probabilitiesA.Length;
            var ratio = new double[n];
            for (int i = 0; i < n; i++)
                ratio[i] = probabilitiesB[i] / probabilitiesA[i];
            return ratio;
        }
    }
}