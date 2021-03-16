using System;
using System.Collections.Concurrent;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// <remarks>
    /// Harrell, F.E. and Davis, C.E., 1982. A new distribution-free quantile estimator. Biometrika, 69(3), pp.635-640.
    /// </remarks>
    /// </summary>
    public class HarrellDavisQuantileEstimator : IQuantileEstimator, IQuantileConfidenceIntervalEstimator
    {
        public static readonly HarrellDavisQuantileEstimator Instance = new HarrellDavisQuantileEstimator();

        public bool SupportsWeightedSamples => true;
        public string Alias => "HD";

        private readonly int cacheSize;
        [CanBeNull] private readonly ConcurrentDictionary<(double a, double b, double x), double> betaCdfCache;

        public HarrellDavisQuantileEstimator(int cacheSize = 10_000)
        {
            Assertion.NonNegative(nameof(cacheSize), cacheSize);
            this.cacheSize = cacheSize;
            betaCdfCache = cacheSize > 0
                ? new ConcurrentDictionary<(double a, double b, double x), double>(1, cacheSize)
                : null;
        }

        public double GetQuantile(Sample sample, Probability probability)
        {
            return GetMoments(sample, probability, false).C1;
        }

        /// <summary>
        /// Estimates confidence intervals using the Maritz-Jarrett method
        /// </summary>
        /// <returns></returns>
        public ConfidenceIntervalEstimator GetQuantileConfidenceIntervalEstimator(Sample sample, Probability probability)
        {
            (double c1, double c2) = GetMoments(sample, probability, true);
            double median = c1;
            double standardError = Math.Sqrt(c2 - c1.Sqr());
            double weightedCount = sample.WeightedCount;
            return new ConfidenceIntervalEstimator(weightedCount, median, standardError);
        }

        private readonly struct Moments
        {
            public readonly double C1;
            public readonly double C2;

            public Moments(double c1, double c2)
            {
                C1 = c1;
                C2 = c2;
            }

            public void Deconstruct(out double c1, out double c2)
            {
                c1 = C1;
                c2 = C2;
            }
        }

        private Moments GetMoments([NotNull] Sample sample, Probability probability, bool calcSecondMoment)
        {
            Assertion.NotNull(nameof(sample), sample);

            double n = sample.WeightedCount;
            double a = (n + 1) * probability, b = (n + 1) * (1 - probability);
            var distribution = new BetaDistribution(a, b);

            double c1 = 0;
            double c2 = calcSecondMoment ? 0 : double.NaN;
            double betaCdfRight = 0;
            double currentProbability = 0;
            for (int j = 0; j < sample.Count; j++)
            {
                double betaCdfLeft = betaCdfRight;
                currentProbability += sample.IsWeighted
                    ? sample.SortedWeights[j] / sample.TotalWeight
                    : 1.0 / sample.Count;

                double cdfValue;
                if (betaCdfCache != null)
                {
                    var key = (a, b, currentProbability);
                    if (betaCdfCache.ContainsKey(key))
                    {
                        cdfValue = betaCdfCache[key];
                    }
                    else
                    {
                        cdfValue = distribution.Cdf(currentProbability);
                        if (betaCdfCache.Count < cacheSize)
                            betaCdfCache[key] = cdfValue;
                    }
                }
                else
                    cdfValue = distribution.Cdf(currentProbability);

                betaCdfRight = cdfValue;
                double w = betaCdfRight - betaCdfLeft;
                c1 += w * sample.SortedValues[j];
                if (calcSecondMoment)
                    c2 += w * sample.SortedValues[j].Sqr();
            }

            return new Moments(c1, c2);
        }
    }
}