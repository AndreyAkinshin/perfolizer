using System;
using System.Collections.Generic;
using System.Linq;
using Perfolizer.Collections;
using Perfolizer.Mathematics.Distributions;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// <remarks>
    /// Harrell, F.E. and Davis, C.E., 1982. A new distribution-free quantile estimator. Biometrika, 69(3), pp.635-640.
    /// </remarks>
    /// </summary>
    public class HarrellDavisQuantileEstimator : IQuantileEstimator, IWeightedQuantileEstimator
    {
        public static readonly IQuantileEstimator Instance = new HarrellDavisQuantileEstimator();

        public double GetQuantile(ISortedReadOnlyList<double> data, double probability)
        {
            QuantileEstimatorHelper.CheckArguments(data, probability);

            return GetQuantile(data, probability, i => 1.0 / data.Count);
        }

        public double GetWeightedQuantile(ISortedReadOnlyList<double> data, IReadOnlyList<double> weights, double probability)
        {
            QuantileEstimatorHelper.CheckWeightedArguments(data, weights, probability);

            double totalWeight = weights.Sum();
            return GetQuantile(data, probability, i => weights[i] / totalWeight);
        }

        private static double GetQuantile(ISortedReadOnlyList<double> data, double probability, Func<int, double> getWeight)
        {
            QuantileEstimatorHelper.CheckArguments(data, probability);

            int n = data.Count;
            double a = (n + 1) * probability, b = (n + 1) * (1 - probability);
            var distribution = new BetaDistribution(a, b);

            double result = 0;
            double betaCdfRight = 0;
            double currentProbability = 0;
            for (int j = 0; j < data.Count; j++)
            {
                double betaCdfLeft = betaCdfRight;
                currentProbability += getWeight(j);
                betaCdfRight = distribution.Cdf(currentProbability);
                result += (betaCdfRight - betaCdfLeft) * data[j];
            }

            return result;
        }
    }
}