using System.Collections.Generic;
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

        protected class TestData
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
                "Case1",
                new TestData(
                    new Range(8, 12),
                    new Range(1.5, 3),
                    new RandomDistribution(1).Gaussian(30, 10, 5),
                    new RandomDistribution(2).Gaussian(30, 20, 5)
                )
            },
            {
                "Case2",
                new TestData(
                    new Range(9.5, 10.5),
                    new Range(1.8, 2.2),
                    new RandomDistribution(1).Gaussian(30, 10, 1),
                    new RandomDistribution(2).Gaussian(30, 20, 1)
                )
            }
        };

        [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void ShiftAndRatioRangeEstimators(string testDataKey)
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