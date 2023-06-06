using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators;

public class HyndmanFanQuantileEstimatorTests : QuantileEstimatorTests
{
    public HyndmanFanQuantileEstimatorTests(ITestOutputHelper output) : base(output)
    {
    }

    public static readonly IDictionary<string, HfTestData> TestDataMap = new Dictionary<string, HfTestData>
    {
        {
            "Type1", new HfTestDataCase1(HyndmanFanType.Type1, new[]
            {
                0.0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                25, 25, 25, 25, 25, 25, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 75, 75, 75, 75,
                75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100
            })
        },
        {
            "Type2", new HfTestDataCase1(HyndmanFanType.Type2, new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12.5, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
                25, 25, 25, 25, 25, 25, 37.5, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 62.5, 75, 75,
                75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 87.5, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100
            })
        },
        {
            "Type3", new HfTestDataCase1(HyndmanFanType.Type3, new[]
            {
                0.0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 25, 25, 25, 25, 25, 25, 25,
                25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50,
                50, 50, 50, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100
            })
        },
        {
            "Type4", new HfTestDataCase1(HyndmanFanType.Type4, new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.25, 2.5, 3.75, 5, 6.25, 7.5, 8.75, 10, 11.25, 12.5,
                13.75, 15, 16.25, 17.5, 18.75, 20, 21.25, 22.5, 23.75, 25, 26.25, 27.5, 28.75, 30, 31.25, 32.5, 33.75, 35, 36.25, 37.5,
                38.75, 40, 41.25, 42.5, 43.75, 45, 46.25, 47.5, 48.75, 50, 51.25, 52.5, 53.75, 55, 56.25, 57.5, 58.75, 60, 61.25, 62.5,
                63.75, 65, 66.25, 67.5, 68.75, 70, 71.25, 72.5, 73.75, 75, 76.25, 77.5, 78.75, 80, 81.25, 82.5, 83.75, 85, 86.25, 87.5,
                88.75, 90, 91.25, 92.5, 93.75, 95, 96.25, 97.5, 98.75, 100
            })
        },
        {
            "Type5", new HfTestDataCase1(HyndmanFanType.Type5, new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.25, 2.5, 3.75, 5, 6.25, 7.5, 8.75, 10, 11.25, 12.5, 13.75, 15, 16.25, 17.5, 18.75,
                20, 21.25, 22.5, 23.75, 25, 26.25, 27.5, 28.75, 30, 31.25, 32.5, 33.75, 35, 36.25, 37.5, 38.75, 40, 41.25, 42.5, 43.75,
                45, 46.25, 47.5, 48.75, 50, 51.25, 52.5, 53.75, 55, 56.25, 57.5, 58.75, 60, 61.25, 62.5, 63.75, 65, 66.25, 67.5, 68.75,
                70, 71.25, 72.5, 73.75, 75, 76.25, 77.5, 78.75, 80, 81.25, 82.5, 83.75, 85, 86.25, 87.5, 88.75, 90, 91.25, 92.5, 93.75,
                95, 96.25, 97.5, 98.75, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100
            })
        },
        {
            "Type6", new HfTestDataCase1(HyndmanFanType.Type6, new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.5, 2, 3.5, 5, 6.5, 8, 9.5, 11, 12.5, 14, 15.5, 17, 18.5, 20, 21.5,
                23, 24.5, 26, 27.5, 29, 30.5, 32, 33.5, 35, 36.5, 38, 39.5, 41, 42.5, 44, 45.5, 47, 48.5, 50, 51.5, 53, 54.5, 56, 57.5,
                59, 60.5, 62, 63.5, 65, 66.5, 68, 69.5, 71, 72.5, 74, 75.5, 77, 78.5, 80, 81.5, 83, 84.5, 86, 87.5, 89, 90.5, 92, 93.5,
                95, 96.5, 98, 99.5, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100
            })
        },
        {
            "Type7", new HfTestDataCase1(HyndmanFanType.Type7, new[]
            {
                0.0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
                32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61,
                62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91,
                92, 93, 94, 95, 96, 97, 98, 99, 100
            })
        },
        {
            "Type8", new HfTestDataCase1(HyndmanFanType.Type8, new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.666666666666671, 2, 3.33333333333333, 4.66666666666667, 6.00000000000001,
                7.33333333333334, 8.66666666666667, 10, 11.3333333333333, 12.6666666666667, 14, 15.3333333333333, 16.6666666666667, 18,
                19.3333333333333, 20.6666666666667, 22, 23.3333333333333, 24.6666666666667, 26, 27.3333333333333, 28.6666666666667, 30,
                31.3333333333333, 32.6666666666667, 34, 35.3333333333333, 36.6666666666667, 38, 39.3333333333333, 40.6666666666667, 42,
                43.3333333333333, 44.6666666666667, 46, 47.3333333333333, 48.6666666666667, 50, 51.3333333333333, 52.6666666666667, 54,
                55.3333333333333, 56.6666666666667, 58, 59.3333333333333, 60.6666666666667, 62, 63.3333333333333, 64.6666666666667, 66,
                67.3333333333333, 68.6666666666667, 70, 71.3333333333333, 72.6666666666667, 74, 75.3333333333333, 76.6666666666667, 78,
                79.3333333333333, 80.6666666666667, 82, 83.3333333333333, 84.6666666666667, 86, 87.3333333333333, 88.6666666666667, 90,
                91.3333333333334, 92.6666666666667, 94, 95.3333333333333, 96.6666666666667, 98, 99.3333333333333, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100
            })
        },
        {
            "Type9", new HfTestDataCase1(HyndmanFanType.Type9, new[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.124999999999997, 1.4375, 2.75, 4.0625, 5.375, 6.6875, 8, 9.3125, 10.625, 11.9375,
                13.25, 14.5625, 15.875, 17.1875, 18.5, 19.8125, 21.125, 22.4375, 23.75, 25.0625, 26.375, 27.6875, 29, 30.3125, 31.625,
                32.9375, 34.25, 35.5625, 36.875, 38.1875, 39.5, 40.8125, 42.125, 43.4375, 44.75, 46.0625, 47.375, 48.6875, 50, 51.3125,
                52.625, 53.9375, 55.25, 56.5625, 57.875, 59.1875, 60.5, 61.8125, 63.125, 64.4375, 65.75, 67.0625, 68.375, 69.6875, 71,
                72.3125, 73.625, 74.9375, 76.25, 77.5625, 78.875, 80.1875, 81.5, 82.8125, 84.125, 85.4375, 86.75, 88.0625, 89.375,
                90.6875, 92, 93.3125, 94.625, 95.9375, 97.25, 98.5625, 99.875, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100
            })
        }
    };

    public abstract class HfTestData
    {
        public HyndmanFanType Type { get; }
        public double[] Expected { get; }
        public abstract double[] Source { get; }
        public abstract Probability[] Probabilities { get; }

        protected HfTestData(HyndmanFanType type, double[] expected)
        {
            Type = type;
            Expected = expected;
        }
    }

    private class HfTestDataCase1 : HfTestData
    {
        public static readonly Probability[] DefaultProbabilities = Enumerable.Range(0, 101)
            .Select(x => (Probability)(x / 100.0)).ToArray();

        public HfTestDataCase1(HyndmanFanType type, double[] expected) : base(type, expected)
        {
        }

        public override double[] Source => new double[] { 0, 25, 50, 75, 100 };
        public override Probability[] Probabilities => DefaultProbabilities;
    }

    [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

    [Theory]
    [MemberData(nameof(TestDataKeys))]
    public void HyndmanFanQuantileEstimatorTest(string testDataKey)
    {
        var data = TestDataMap[testDataKey];
        var estimator = new HyndmanFanQuantileEstimator(data.Type);
        Check(estimator, new TestData(data.Source, data.Probabilities, data.Expected));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(4, 10)]
    [InlineData(5, 20)]
    [InlineData(6, 30)]
    public void HyndmanFanQuantileEstimatorWeightedTest1(int seed, int n)
    {
        var types = HyndmanFanHelper.AllTypes.Where(HyndmanFanHelper.SupportsWeightedSamples).ToArray();
        var sample = NormalDistribution.Standard.Random(seed).Next(n).ToSample();
        var probabilities = HfTestDataCase1.DefaultProbabilities;
        foreach (var type in types)
        {
            var estimator = new HyndmanFanQuantileEstimator(type);
            double[] expected = estimator.Quantiles(sample, probabilities);
            Check(estimator, new TestData(sample.Values.ToArray(), probabilities, expected));
        }
    }

    [Fact]
    public void HyndmanFanQuantileEstimatorWeightedTest2()
    {
        Check(SimpleQuantileEstimator.Instance, new TestData(
            new double[] { 1, 1.5, 2, 2.5, 3, 2, 2.5, 3, 3.5, 3, 3.5, 4, 4, 4.5, 5 },
            new[] { Probability.Half },
            new double[] { 2 },
            new double[] { 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0 }));
    }
}