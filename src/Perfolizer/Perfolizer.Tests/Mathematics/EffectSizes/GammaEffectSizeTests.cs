using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.EffectSizes;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Tests.Common;

namespace Perfolizer.Tests.Mathematics.EffectSizes;

public class GammaEffectSizeTests
{
    private readonly ITestOutputHelper output;

    public GammaEffectSizeTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(2.5)]
    [InlineData(10.0)]
    [Trait(TraitConstants.Category, TraitConstants.Slow)]
    public void GammaCohenDConsistencyTest(double stdDev)
    {
        const int n = 100;
        const double delta = 1.0;
        var random = new Random(42);
        var diffs = new double[n];
        output.WriteLine("mean(X)-mean(Y) : " + delta);
        output.WriteLine("StandardDeviation : " + stdDev);

        var gammaEffectSizeFunction = GammaEffectSizeFunction.Instance;
        for (int i = 0; i < n; i++)
        {
            var x = new Sample(new NormalDistribution(0, 1).Random(random).Next(200));
            var y = new Sample(new NormalDistribution(delta, 1).Random(random).Next(200));
            var gamma = gammaEffectSizeFunction.Range(x, y, 0.5);
            double d = CohenDEffectSize.Instance.EffectSize(x, y);
            diffs[i] = Math.Abs(d - gamma.Middle);
        }

        Array.Sort(diffs);
        output.WriteLine("*** Sorted Abs(Gamma-d) ***");
        for (int i = 0; i < diffs.Length; i++)
            output.WriteLine($"{i}: {diffs[i]:0.0000}");
        output.WriteLine("");

        void Check(int percentile, double expectedMax)
        {
            double diff = diffs[(n - 1) * percentile / 100];
            output.WriteLine($"{percentile}%: {diff:0.0000} (Max = {expectedMax:0.00})");
            Assert.True(diff < expectedMax);
        }

        Check(10, 0.02);
        Check(20, 0.03);
        Check(30, 0.04);
        Check(40, 0.05);
        Check(50, 0.07);
        Check(60, 0.08);
        Check(70, 0.10);
        Check(80, 0.13);
        Check(90, 0.16);
        Check(100, 0.40);
    }

    [Fact]
    public void GammaEffectSizeMultimodalTest()
    {
        var random = new Random(42);
        var x = new NormalDistribution(0, 1).Random(random).Next(200)
            .Concat(new NormalDistribution(10, 1).Random(random).Next(200))
            .ToSample();
        var y = new NormalDistribution(0, 1).Random(random).Next(200)
            .Concat(new NormalDistribution(20, 1).Random(random).Next(200))
            .ToSample();

        var probabilities = new Probability[] {0.2, 0.3, 0.4, 0.6, 0.7, 0.8};
        var minExpected = new[] {-0.1, -0.1, -0.1, 0.8, 0.8, 0.8};
        var maxExpected = new[] {0.1, 0.1, 0.1, 0.9, 0.9, 0.9};

        var gammaEffectSizeFunction = GammaEffectSizeFunction.Instance;
        for (int i = 0; i < probabilities.Length; i++)
        {
            var p = probabilities[i];
            double gamma = gammaEffectSizeFunction.Value(x, y, p);
            output.WriteLine($"{p}: {gamma:0.0000}");
            Assert.True(minExpected[i] < gamma && gamma < maxExpected[i]);
        }
    }

    private class TestData
    {
        public double[] X { get; }
        public double[] Y { get; }
        public Probability Probability { get; }
        public double ExpectedGammaEffectSize { get; }

        public TestData(double[] x, double[] y, Probability probability, double expectedGammaEffectSize)
        {
            X = x;
            Y = y;
            Probability = probability;
            ExpectedGammaEffectSize = expectedGammaEffectSize;
        }
    }

    private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
    {
        {"Corner/3-1", new TestData(new []{1.0, 2.0, 3.0}, new[] {2.0}, 0.5, 0.0)},
        {"Corner/1-3", new TestData(new []{2.0}, new[] {1.0, 2.0, 3.0}, 0.5, 0.0)},
        {"Corner/1-1/Same", new TestData(new []{2.0}, new[] {2.0}, 0.5, 0.0)},
        {"Corner/1-1/Less", new TestData(new []{2.0}, new[] {3.0}, 0.5, double.PositiveInfinity)},
        {"Corner/Identical-Identical/Same", new TestData(new []{1.0, 1.0}, new[] {1.0, 1.0}, 0.5, 0.0)},
        {"Corner/Identical-Identical/Less", new TestData(new []{1.0, 1.0}, new[] {2.0, 2.0}, 0.5, double.PositiveInfinity)},
        {"Corner/Identical-Identical/More", new TestData(new []{2.0, 2.0}, new[] {1.0, 1.0}, 0.5, double.NegativeInfinity)},
    };

    [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

    [Theory]
    [MemberData(nameof(TestDataKeys))]
    public void GammaEffectSizeGeneralTest(string testKey)
    {
        var testData = TestDataMap[testKey];
        var x = testData.X.ToSample();
        var y = testData.Y.ToSample();
        double expected = testData.ExpectedGammaEffectSize;
        double actual = GammaEffectSizeFunction.Instance.Value(x, y, testData.Probability);
        Assert.Equal(expected, actual);
    }
}