using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Randomization;
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

        private class TestData
        {
            public int Seed { get; }
            public Probability Probability { get; }
            public int N { get; }
            public bool Randomize { get; }
            public IDistribution Distribution { get; }

            public TestData(int seed, Probability probability, int n, bool randomize, IDistribution distribution)
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

        private static readonly IDictionary<string, TestData> TestDataMap;
        [UsedImplicitly] public static TheoryData<string> TestDataKeys;

        static P2QuantileEstimatorTests()
        {
            TestDataMap = new Dictionary<string, TestData>();
            var distributions = new IDistribution[]
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
            
            double actual = estimator.GetQuantile();
            double expected = SimpleQuantileEstimator.Instance.GetQuantile(sample, p);
            double pDelta = 0.1 + Math.Abs(p - 0.5) * 0.2 + 2.0 / testData.N;
            double expectedMin = SimpleQuantileEstimator.Instance.GetQuantile(sample, (p - pDelta).Clamp(0, 1));
            double expectedMax = SimpleQuantileEstimator.Instance.GetQuantile(sample, (p + pDelta).Clamp(0, 1));
            double mad = MedianAbsoluteDeviation.CalcMad(sample);
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

            double actual = estimator.GetQuantile();
            const double expected = 4.44;

            Assert.Equal(expected, actual, new AbsoluteEqualityComparer(1e-2));
        }
    }
}