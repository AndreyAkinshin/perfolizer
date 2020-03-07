using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Randomization;
using Perfolizer.Mathematics.RangeEstimators;
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
            public double[] A { get; }
            public double[] B { get; }

            public TestData(Range expectedShift, Range expectedRatio, double[] a, double[] b)
            {
                ExpectedShift = expectedShift;
                ExpectedRatio = expectedRatio;
                A = a;
                B = b;
            }
        }

        private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
        {
            {
                "Equal",
                new TestData(
                    Range.Of(0, 00),
                    Range.Of(1, 1),
                    new RandomDistribution(1).Gaussian(30, 10, 5),
                    new RandomDistribution(1).Gaussian(30, 10, 5)
                )
            },
            {
                "Unimodal1",
                new TestData(
                    Range.Of(8, 12),
                    Range.Of(1.5, 3),
                    new RandomDistribution(1).Gaussian(30, 10, 5),
                    new RandomDistribution(2).Gaussian(30, 20, 5)
                )
            },
            {
                "Unimodal2",
                new TestData(
                    Range.Of(9.5, 10.5),
                    Range.Of(1.8, 2.2),
                    new RandomDistribution(1).Gaussian(30, 10),
                    new RandomDistribution(2).Gaussian(30, 20)
                )
            },
            {
                "Unimodal3",
                new TestData(
                    Range.Of(96, 104),
                    Range.Of(1.96, 2.04),
                    new RandomDistribution(1).Gaussian(20, 100, 5),
                    new RandomDistribution(2).Gaussian(20, 200, 10)
                )
            },
            {
                "Bimodal1",
                new TestData(
                    Range.Of(-0.5, 40.5),
                    Range.Of(0.98, 2.02),
                    new RandomDistribution(1).Gaussian(20, 20, 2).Concat(new RandomDistribution(2).Gaussian(20, 40, 2)).ToArray(),
                    new RandomDistribution(3).Gaussian(20, 20, 2).Concat(new RandomDistribution(4).Gaussian(20, 80, 2)).ToArray()
                )
            },
            {
                "Bimodal2",
                new TestData(
                    Range.Of(-20.5, 0.5),
                    Range.Of(0.45, 1.05),
                    new RandomDistribution(1).Gaussian(20, 80, 2).Concat(new RandomDistribution(2).Gaussian(20, 40, 2)).ToArray(),
                    new RandomDistribution(3).Gaussian(20, 80, 2).Concat(new RandomDistribution(4).Gaussian(20, 20, 2)).ToArray()
                )
            },
            {
                "Bimodal3",
                new TestData(
                    Range.Of(-10.5, 40.5),
                    Range.Of(0.45, 2.05),
                    new RandomDistribution(1).Gaussian(20, 20, 2).Concat(new RandomDistribution(2).Gaussian(20, 40, 2)).ToArray(),
                    new RandomDistribution(3).Gaussian(20, 10, 2).Concat(new RandomDistribution(4).Gaussian(20, 80, 2)).ToArray()
                )
            }
        };

        [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void ShiftAndRatioRangeEstimators([NotNull] string testDataKey)
        {
            var testData = TestDataMap[testDataKey];
            var shift = ShiftRangeEstimator.Instance.GetRange(testData.A, testData.B);
            var ratio = RatioRangeEstimator.Instance.GetRange(testData.A, testData.B);
            output.WriteLine($"Shift: {shift}");
            output.WriteLine($"Ratio: {ratio}");
            Assert.True(shift.IsInside(testData.ExpectedShift));
            Assert.True(ratio.IsInside(testData.ExpectedRatio));
        }
    }
}