using System.Collections.Generic;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.DiscreteDistributions;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public abstract class BinomialBasedQuantileEstimator : IQuantileEstimator
    {
        public double GetQuantile(Sample sample, Probability probability)
        {
            Assertion.NonWeighted(nameof(sample), sample);

            int n = sample.Count;
            if (n <= 2)
                return SimpleQuantileEstimator.Instance.GetQuantile(sample, probability);

            var distribution = new BinomialDistribution(n, probability);
            double[] b = new double[n + 1];
            for (int k = 0; k <= n; k++)
                b[k] = distribution.Pmf(k);

            return GetQuantile(sample.SortedValues, probability, b);
        }

        public bool SupportsWeightedSamples => false;

        protected abstract double GetQuantile(IReadOnlyList<double> x, Probability probability, double[] b);
    }
}