using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.OutlierDetection;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.OutlierDetection
{
    public class TukeyOutlierDetectorTests : OutlierDetectorTests
    {
        public TukeyOutlierDetectorTests([NotNull] ITestOutputHelper output) : base(output)
        {
        }

        protected override IOutlierDetector CreateOutlierDetector(IReadOnlyList<double> values) =>
            TukeyOutlierDetector.Create(values, quantileEstimator: SimpleQuantileEstimator.Instance);

        private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
        {
            {"Yang_X0", new TestData(YangDataSet.X0, new double[] { })},
            {"Yang_X1", new TestData(YangDataSet.X1, new double[] {1000})},
            {"Yang_X2", new TestData(YangDataSet.X2, new double[] {1000})},
            {"Yang_X3", new TestData(YangDataSet.X3, new double[] {60, 1000})},
            {"Yang_X4", new TestData(YangDataSet.X4, new double[] {1000, 1500})}
        };

        [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void TukeyOutlierDetectorTest([NotNull] string testDataKey) => Check(TestDataMap[testDataKey]);
    }
}