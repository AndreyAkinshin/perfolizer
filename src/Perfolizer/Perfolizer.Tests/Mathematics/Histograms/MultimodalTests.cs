using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Histograms;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Mathematics.OutlierDetection;
using Perfolizer.Tests.Common;

namespace Perfolizer.Tests.Mathematics.Histograms;

public class MultimodalTests
{
    private readonly ITestOutputHelper output;

    public MultimodalTests(ITestOutputHelper output) => this.output = output;

    private class TestData
    {
        public double[] Values { get; }
        public double ExpectedMValue { get; }

        public TestData(double[] values, double expectedMValue)
        {
            Values = values;
            ExpectedMValue = expectedMValue;
        }
    }

    private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
    {
        {"Case1", new TestData(new double[] {1, 1, 1, 1, 1, 1}, 2)},
        {"Case2", new TestData(new double[] {1, 1, 1, 1, 1, 2, 2, 2, 2, 2}, 4)},
        {"Case3", new TestData(new double[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3}, 6)},
        {"Case4", new TestData(new double[] {1, 2, 3, 3, 3, 4, 5, 10, 11, 11, 11, 12, 40, 41, 41, 41, 42}, 2.8333)},
        {
            "Case5", new TestData(new double[]
            {
                173, 147, 153, 141, 139, 117, 126, 145, 133, 181, 166, 139,
                138, 146, 145, 142, 121, 108, 128, 122, 149, 118, 113, 117, 129,
                148, 124, 141, 143, 109, 144, 106, 98, 116, 129, 136, 131, 179,
                115, 142, 152, 148, 133, 138, 114, 142, 151, 151, 144, 160, 151,
                134, 133, 135, 109, 126, 123, 140, 141, 136, 122, 134, 148, 141,
                133, 132, 166, 134, 112, 158, 135, 95, 141, 113, 105, 137, 150,
                152, 151, 114, 142, 132, 112, 159, 119, 141, 129, 122, 102, 135,
                137, 147, 131, 107, 135, 133, 120, 166, 132, 122, 144, 127
            }, 2.4516)
        },
        {
            "Case6", new TestData(new double[]
            {
                1173, 147, 153, 141, 139, 117, 126, 145, 133, 181, 166, 139, 138, 148, 124, 141, 143, 109, 144, 106, 98, 116, 129,
                136, 131, 179, 115, 142, 152, 197, 148, 133, 138, 114, 196, 142, 151, 151, 144, 160, 151, 134, 133, 135, 109, 126,
                123, 140, 141, 136, 219, 122, 134, 148, 226, 199, 221, 141, 132, 133, 166, 134, 112, 158, 135, 95, 141, 113, 105,
                189, 150, 137, 152, 151, 114, 142, 132, 112, 132, 122, 144, 127
            }, 2.2143)
        },
    };

    [UsedImplicitlyAttribute] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

    [Theory]
    [MemberData(nameof(TestDataKeys))]
    public void MValueTest(string testDataKey)
    {
        var testData = TestDataMap[testDataKey];
        var values = testData.Values;
        double expectedMValue = testData.ExpectedMValue;

        double actualMValue = MValueCalculator.Calculate(values);

        var histogram = HistogramBuilder.Adaptive.Build(values);
        output.Print("Distribution", histogram);

        var clearedValues = TukeyOutlierDetector.Create(values).WithoutAllOutliers(values).ToList();
        var clearedHistogram = HistogramBuilder.Adaptive.Build(clearedValues);
        output.Print("ClearedDistribution", clearedHistogram);

        Assert.Equal(expectedMValue, actualMValue, 4);
    }

    [Fact]
    [Trait(TraitConstants.Category, TraitConstants.Slow)]
    public void RandomTest()
    {
        var normalDistributionRandom = new NormalDistribution(mean: 50, stdDev: 3).Random(42);
        double maxMValue = 0;
        int maxMValueN = 0;
        for (int n = 1; n <= 300; n++)
        {
            var values = normalDistributionRandom.Next(n);

            var histogram = HistogramBuilder.Adaptive.Build(values);
            double mValue = MValueCalculator.Calculate(values);
            output.Print($"n={n}", histogram);
            output.WriteLine($"MValue = {mValue:N4}");
            output.WriteLine("-------------------------------------------------------------------------");

            Assert.True(mValue >= 2 - 1e-9);

            if (mValue > maxMValue)
            {
                maxMValue = mValue;
                maxMValueN = n;
            }
        }

        output.WriteLine($"maxMValue = {maxMValue} (N = {maxMValueN})");
    }
}