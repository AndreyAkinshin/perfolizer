using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Randomization;
using Perfolizer.Mathematics.ScaleEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public class P2QuantileEstimatorTests
    {
        private readonly ITestOutputHelper output;

        public P2QuantileEstimatorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        internal class TestData
        {
            public int Seed { get; }
            public Probability Probability { get; }
            public int N { get; }
            public bool Randomize { get; }
            public IContinuousDistribution Distribution { get; }

            public TestData(int seed, Probability probability, int n, bool randomize, IContinuousDistribution distribution)
            {
                Seed = seed;
                Probability = probability;
                N = n;
                Randomize = randomize;
                Distribution = distribution;
            }

            public Sample Generate()
            {
                var random = new Random(Seed);
                if (Randomize)
                    return new Sample(Distribution.Random(random).Next(N));
                
                double[] values = Enumerable.Range(0, N)
                    .Select(x => (x + 1.0) / (N + 1))
                    .Select(x => Distribution.Quantile(x))
                    .ToArray();
                new Shuffler(random).Shuffle(values);
                return new Sample(values);
            }
        }

        internal static readonly IDictionary<string, TestData> TestDataMap;
        [UsedImplicitly] public static TheoryData<string> TestDataKeys;

        static P2QuantileEstimatorTests()
        {
            TestDataMap = new Dictionary<string, TestData>();
            var distributions = new IContinuousDistribution[]
            {
                new UniformDistribution(0, 1),
                new NormalDistribution(0, 1),
                new GumbelDistribution()
            };
            var seedMap = new Dictionary<bool, int[]>
            {
                {true, Enumerable.Range(0, 5).ToArray()},
                {false, new[] {0}}
            };
            foreach (bool randomize in new[] {false, true})
            foreach (int seed in seedMap[randomize])
            foreach (Probability probability in new Probability[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9})
            foreach (int n in new[] {5, 10, 100, 1000})
            foreach (var distribution in distributions)
            {
                string name = distribution.GetType().Name.Replace("Distribution", "") +
                              "/" +
                              "P" + (probability * 100) +
                              "/" +
                              "N" + n +
                              "/" +
                              (randomize ? "Random" : "Perfect") +
                              "/#" + seed;
                TestDataMap[name] = new TestData(seed, probability, n, randomize, distribution);
            }
            TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

            TestStrategyDataMap = new Dictionary<string, TestData>();
            foreach (int seed in new[] {1, 2, 3})
            foreach (Probability probability in new Probability[] {0.05, 0.1, 0.2, 0.8, 0.9, 0.95})
            foreach (int n in new[] {6, 7, 8})
            foreach (var distribution in distributions)
            {
                string name = distribution.GetType().Name.Replace("Distribution", "") +
                              "/" +
                              "P" + (probability * 100) +
                              "/" +
                              "N" + n +
                              "/#" + seed;
                TestStrategyDataMap[name] = new TestData(seed, probability, n, true, distribution);
            }
            TestStrategyDataKeys = TheoryDataHelper.Create(TestStrategyDataMap.Keys);
        }
        

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void P2QuantileEstimatorTest(string testKey)
        {
            var testData = TestDataMap[testKey];
            double p = testData.Probability;
            var sample = testData.Generate();
            var estimator = new P2QuantileEstimator(p);
            foreach (double x in sample.Values)
                estimator.Add(x);
            
            double actual = estimator.Quantile();
            double expected = SimpleQuantileEstimator.Instance.Quantile(sample, p);
            double pDelta = 0.1 + Math.Abs(p - 0.5) * 0.15 + 1.5 / testData.N;
            double expectedMin = SimpleQuantileEstimator.Instance.Quantile(sample, (p - pDelta).Clamp(0, 1));
            double expectedMax = SimpleQuantileEstimator.Instance.Quantile(sample, (p + pDelta).Clamp(0, 1));
            double mad = MedianAbsoluteDeviationEstimator.Simple.Mad(sample);
            double error = Math.Abs(actual - expected);
            double errorNorm = error / mad;

            output.WriteLine($"ExpectedMin = {expectedMin:N5}");
            output.WriteLine($"Expected    = {expected:N5}");
            output.WriteLine($"ExpectedMax = {expectedMax:N5}");
            output.WriteLine($"Actual      = {actual:N5}");
            output.WriteLine($"Error       = {error:N5}");
            output.WriteLine($"ErrorNorm   = {errorNorm:N5}");

            Assert.True(expectedMin <= actual && actual <= expectedMax);
        }

        /// <summary>
        /// This test corresponds to Table I from the original paper:
        /// Jain, Raj, and Imrich Chlamtac.
        /// "The P2 algorithm for dynamic calculation of quantiles and histograms without storing observations."
        /// Communications of the ACM 28, no. 10 (1985): 1076-1085.
        /// https://doi.org/10.1145/4372.4378
        /// </summary>
        [Fact]
        public void P2QuantileEstimatorReferenceTest()
        {
            double[] values =
            {
                0.02, 0.15, 0.74, 3.39, 0.83, 22.37, 10.15, 15.43, 38.62, 15.92,
                34.60, 10.28, 1.47, 0.40, 0.05, 11.39, 0.27, 0.42, 0.09, 11.37
            };
            var estimator = new P2QuantileEstimator(0.5);
            foreach (double x in values)
                estimator.Add(x);

            double actual = estimator.Quantile();
            const double expected = 4.44;

            Assert.Equal(expected, actual, new AbsoluteEqualityComparer(1e-2));
        }
        
        internal static readonly IDictionary<string, TestData> TestStrategyDataMap;
        [UsedImplicitly] public static TheoryData<string> TestStrategyDataKeys;

        [Theory]
        [MemberData(nameof(TestStrategyDataKeys))]
        public void P2QuantileEstimatorStrategyTest(string testKey)
        {
            var testData = TestStrategyDataMap[testKey];
            var random = new Random(testData.Seed);
            var randomGenerator = testData.Distribution.Random(random);
            var probability = testData.Probability;

            const int totalIterations = 1_000;
            int classicIsWinner = 0;
            for (int iteration = 0; iteration < totalIterations; iteration++)
            {
                var p2ClassicEstimator = new P2QuantileEstimator(probability, P2QuantileEstimator.InitializationStrategy.Classic);
                var p2AdaptiveEstimator = new P2QuantileEstimator(probability, P2QuantileEstimator.InitializationStrategy.Adaptive);
                var values = new List<double>();
                for (int i = 0; i < testData.N; i++)
                {
                    double x = randomGenerator.Next();
                    values.Add(x);
                    p2ClassicEstimator.Add(x);
                    p2AdaptiveEstimator.Add(x);
                }

                double simpleEstimation = SimpleQuantileEstimator.Instance.Quantile(values, probability);
                double p2ClassicEstimation = p2ClassicEstimator.Quantile();
                double p2AdaptiveEstimation = p2AdaptiveEstimator.Quantile();
                if (Math.Abs(p2ClassicEstimation - simpleEstimation) < Math.Abs(p2AdaptiveEstimation - simpleEstimation))
                    classicIsWinner++;
            }

            int adaptiveIsWinner = totalIterations - classicIsWinner;
            double factor = testData.N switch
            {
                6 => 0.2,
                7 => 0.29,
                8 => 0.42,
                _ => throw new NotSupportedException()
            };
            int classicIsWinnerThreshold = (int)Math.Round(totalIterations * factor); 
            
            output.WriteLine("ClassicIsWinner  : {0} (Threshold: {1})", classicIsWinner, classicIsWinnerThreshold);
            output.WriteLine("AdaptiveIsWinner : {0}", adaptiveIsWinner);
            Assert.True(classicIsWinner < classicIsWinnerThreshold);
        }
    }
}