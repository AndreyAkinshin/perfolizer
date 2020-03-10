using System;
using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Functions
{
    public abstract class DistributionCompareFunction
    {
        [NotNull] private readonly IQuantileEstimator quantileEstimator;

        public DistributionCompareFunction([NotNull] IQuantileEstimator quantileEstimator)
        {
            this.quantileEstimator = quantileEstimator;
        }
        
        public DistributionCompareFunction()
        {
            quantileEstimator = HarrellDavisQuantileEstimator.Instance;
        }

        public double[] Values([NotNull] double[] a, [NotNull] double[] b, [NotNull] double[] probabilities)
        {
            for (int i = 0; i < probabilities.Length; i++)
                if (probabilities[i] < 0 || probabilities[i] > 1)
                    throw new ArgumentOutOfRangeException(nameof(probabilities),
                        $"{nameof(probabilities)}[{i}] = {probabilities[i]}, but it should be inside [0;1]");

            var qa = quantileEstimator.GetQuantiles(a, probabilities);
            var qb = quantileEstimator.GetQuantiles(b, probabilities);
            return CalculateValues(qa, qb);
        }

        protected abstract double[] CalculateValues(double[] probabilitiesA, double[] probabilitiesB);
    }
}