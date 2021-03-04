using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.RangeEstimator
{
    public class RangeEstimatorTests
    {
        private readonly ITestOutputHelper output;

        public RangeEstimatorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private class TestData
        {
            public Range ExpectedShift { get; }
            public Range ExpectedRatio { get; }
            public Sample A { get; }
            public Sample B { get; }

            public TestData(Range expectedShift, Range expectedRatio, double[] a, double[] b)
            {
                ExpectedShift = expectedShift;
                ExpectedRatio = expectedRatio;
                A = a.ToSample();
                B = b.ToSample();
            }
        }

        private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
        {
            {
                "Equal",
                new TestData(
                    Range.Of(0, 00),
                    Range.Of(1, 1),
                    new NormalDistribution(mean: 10, stdDev: 5).Random(1).Next(30),
                    new NormalDistribution(mean: 10, stdDev: 5).Random(1).Next(30)
                )
            },
            {
                "Unimodal1",
                new TestData(
                    Range.Of(8, 12),
                    Range.Of(1.5, 3),
                    new NormalDistribution(mean: 10, stdDev: 5).Random(1).Next(30),
                    new NormalDistribution(mean: 20, stdDev: 5).Random(2).Next(30)
                )
            },
            {
                "Unimodal2",
                new TestData(
                    Range.Of(9.5, 10.5),
                    Range.Of(1.8, 2.2),
                    new NormalDistribution(mean: 10, stdDev: 1).Random(1).Next(30),
                    new NormalDistribution(mean: 20, stdDev: 1).Random(2).Next(30)
                )
            },
            {
                "Unimodal3",
                new TestData(
                    Range.Of(96, 104),
                    Range.Of(1.96, 2.04),
                    new NormalDistribution(mean: 100, stdDev: 5).Random(1).Next(20),
                    new NormalDistribution(mean: 200, stdDev: 10).Random(2).Next(20)
                )
            },
            {
                "Bimodal1",
                new TestData(
                    Range.Of(-0.5, 40.5),
                    Range.Of(0.98, 2.02),
                    new NormalDistribution(mean: 20, stdDev: 2).Random(1).Next(20).Concat(
                        new NormalDistribution(mean: 40, stdDev: 2).Random(2).Next(20)).ToArray(),
                    new NormalDistribution(mean: 20, stdDev: 2).Random(3).Next(20).Concat(
                        new NormalDistribution(mean: 80, stdDev: 2).Random(4).Next(20)).ToArray()
                )
            },
            {
                "Bimodal2",
                new TestData(
                    Range.Of(-20.5, 0.5),
                    Range.Of(0.45, 1.05),
                    new NormalDistribution(mean: 80, stdDev: 2).Random(1).Next(20).Concat(
                        new NormalDistribution(mean: 40, stdDev: 2).Random(2).Next(20)).ToArray(),
                    new NormalDistribution(mean: 80, stdDev: 2).Random(3).Next(20).Concat(
                        new NormalDistribution(mean: 20, stdDev: 2).Random(4).Next(20)).ToArray()
                )
            },
            {
                "Bimodal3",
                new TestData(
                    Range.Of(-10.5, 40.5),
                    Range.Of(0.45, 2.05),
                    new NormalDistribution(mean: 20, stdDev: 2).Random(1).Next(20).Concat(
                        new NormalDistribution(mean: 40, stdDev: 2).Random(2).Next(20)).ToArray(),
                    new NormalDistribution(mean: 10, stdDev: 2).Random(3).Next(20).Concat(
                        new NormalDistribution(mean: 80, stdDev: 2).Random(4).Next(20)).ToArray()
                )
            }
        };

        [UsedImplicitly]
        public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void ShiftAndRatioRangeEstimators([NotNull] string testDataKey)
        {
            var testData = TestDataMap[testDataKey];
            var shift = ShiftFunction.Instance.GetRange(testData.A, testData.B);
            var ratio = RatioFunction.Instance.GetRange(testData.A, testData.B);
            output.WriteLine($"Shift: {shift}");
            output.WriteLine($"Ratio: {ratio}");
            Assert.True(shift.IsInside(testData.ExpectedShift));
            Assert.True(ratio.IsInside(testData.ExpectedRatio));
        }
    }
}