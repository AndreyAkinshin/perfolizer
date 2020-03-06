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

        public double[] Values(double[] a, double[] b, double[] quantiles)
        {
            for (int i = 0; i < quantiles.Length; i++)
                if (quantiles[i] < 0 || quantiles[i] > 1)
                    throw new ArgumentOutOfRangeException(nameof(quantiles),
                        $"{nameof(quantiles)}[{i}] = {quantiles[i]}, but it should be inside [0;1]");

            var qa = quantileEstimator.GetQuantiles(a, quantiles);
            var qb = quantileEstimator.GetQuantiles(b, quantiles);
            return CalculateValues(qa, qb);
        }

        protected abstract double[] CalculateValues(double[] quantilesA, double[] quantilesB);
    }
}