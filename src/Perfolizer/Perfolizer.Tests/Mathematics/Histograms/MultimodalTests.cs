using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.Histograms;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Histograms
{
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
            {"Case3", new TestData(new double[] {1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3}, 6)},
            {"Case4", new TestData(new double[] {1, 2, 3, 3, 3, 4, 5, 10, 11, 11, 11, 12, 40, 41, 41, 41, 42}, 2.8333)}
        };

        [UsedImplicitlyAttribute] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void MValueTest([NotNull] string testDataKey)
        {
            var testData = TestDataMap[testDataKey];
            var values = testData.Values;
            double expectedMValue = testData.ExpectedMValue;

            var histogram = HistogramBuilder.Adaptive.Build(values);
            output.Print("Distribution", histogram);

            double actualMValue = MValueCalculator.Calculate(values);
            Assert.Equal(expectedMValue, actualMValue, 4);
        }

        [Fact]
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
}