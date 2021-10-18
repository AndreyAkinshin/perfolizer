using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;
using Range = Perfolizer.Mathematics.Common.Range;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public class SmokeQuantileEstimatorTests
    {
        private readonly ITestOutputHelper output;

        public SmokeQuantileEstimatorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private class TestData
        {
            public string Key { get; }
            public IContinuousDistribution Distribution { get; }
            public IQuantileEstimator Estimator { get; }
            public int SampleSize { get; }
            public Probability P { get; }

            public TestData(string key, IContinuousDistribution distribution, IQuantileEstimator estimator, int sampleSize, Probability p)
            {
                Key = key;
                Distribution = distribution;
                Estimator = estimator;
                SampleSize = sampleSize;
                P = p;
            }

            public void Deconstruct(out IContinuousDistribution distribution, out IQuantileEstimator estimator, out int sampleSize,
                out Probability p)
            {
                distribution = Distribution;
                estimator = Estimator;
                sampleSize = SampleSize;
                p = P;
            }
        }

        private static readonly List<TestData> TestDataList = new();

        [UsedImplicitly] public static TheoryData<string> TestDataKeys;

        static SmokeQuantileEstimatorTests()
        {
            var distributions = new List<(string Name, IContinuousDistribution distribution)>
            {
                ("Normal", NormalDistribution.Standard),
                ("Gumbel", new GumbelDistribution())
            };
            var estimators = new List<(string Name, IQuantileEstimator estimator)>
            {
                ("HF1", new HyndmanFanQuantileEstimator(HyndmanFanType.Type1)),
                ("HF2", new HyndmanFanQuantileEstimator(HyndmanFanType.Type2)),
                ("HF3", new HyndmanFanQuantileEstimator(HyndmanFanType.Type3)),
                ("HF4", new HyndmanFanQuantileEstimator(HyndmanFanType.Type4)),
                ("HF5", new HyndmanFanQuantileEstimator(HyndmanFanType.Type5)),
                ("HF6", new HyndmanFanQuantileEstimator(HyndmanFanType.Type6)),
                ("HF7", new HyndmanFanQuantileEstimator(HyndmanFanType.Type7)),
                ("HF8", new HyndmanFanQuantileEstimator(HyndmanFanType.Type8)),
                ("HF9", new HyndmanFanQuantileEstimator(HyndmanFanType.Type9)),
                ("HD", HarrellDavisQuantileEstimator.Instance),
                ("THD", TrimmedHarrellDavisQuantileEstimator.SqrtInstance),
                ("SV1", SfakianakisVerginis1QuantileEstimator.Instance),
                ("SV2", SfakianakisVerginis2QuantileEstimator.Instance),
                ("SV3", SfakianakisVerginis3QuantileEstimator.Instance),
                ("NO", NavruzOzdemirQuantileEstimator.Instance),
            };
            int[] sampleSizes = {10, 50, 100};
            var probabilities = new Probability[] {0.25, 0.5, 0.75};

            foreach (var (distributionName, distribution) in distributions)
            foreach (var (estimatorName, estimator) in estimators)
            foreach (int sampleSize in sampleSizes)
            foreach (var probability in probabilities)
            {
                string key = $"{distributionName}/{estimatorName}/N{sampleSize}/P{probability}";
                TestDataList.Add(new TestData(key, distribution, estimator, sampleSize, probability));
            }

            TestDataKeys = TheoryDataHelper.Create(TestDataList.Select(it => it.Key));
        }

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void SmokeQuantileEstimatorTest(string testKey)
        {
            var testData = TestDataList.First(it => it.Key == testKey);
            (var distribution, var estimator, int sampleSize, var p) = testData;
            double trueValue = distribution.Quantile(p);
            double trueValueVar = (p * (1 - p) / sampleSize / distribution.Pdf(trueValue).Sqr()).Sqrt();
            var expectedRange = Range.Of(trueValue - trueValueVar * 2, trueValue + trueValueVar * 2);
            output.WriteLine("True value: " + trueValue.ToStringInvariant());
            output.WriteLine("Expected range: " + expectedRange);
            output.WriteLine("");

            var randomGenerator = distribution.Random(42);
            const int totalIterations = 100;
            int hitCount = 0;
            for (int iteration = 0; iteration < totalIterations; iteration++)
            {
                var sample = new Sample(randomGenerator.Next(sampleSize));
                double estimation = estimator.GetQuantile(sample, p);
                output.WriteLine($"Estimation#{iteration} = {estimation.ToStringInvariant()}");

                if (expectedRange.ContainsInclusive(estimation))
                    hitCount++;
            }
            double successRate = hitCount * 1.0 / totalIterations;

            output.WriteLine("");
            output.WriteLine($"SuccessRate = {successRate.ToStringInvariant()}");

            Assert.True(successRate > 0.8);
        }
    }
}