using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.OutlierDetection;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.OutlierDetection
{
    public class MadOutlierDetectorTests : OutlierDetectorTests
    {
        private static readonly IQuantileEstimator QuantileEstimator = SimpleQuantileEstimator.Instance;

        public MadOutlierDetectorTests([NotNull] ITestOutputHelper output) : base(output)
        {
        }

        protected override IOutlierDetector CreateOutlierDetector(IReadOnlyList<double> values) => MadOutlierDetector.Create(values,
            quantileEstimator: QuantileEstimator);

        private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
        {
            {"Yang_X0", new TestData(YangDataSet.X0, new double[] { })},
            {"Yang_X1", new TestData(YangDataSet.X1, new double[] {1000})},
            {"Yang_X2", new TestData(YangDataSet.X2, new double[] {500, 1000})},
            {"Yang_X3", new TestData(YangDataSet.X3, new double[] {1000})},
            {"Yang_X4", new TestData(YangDataSet.X4, new double[] {300, 500, 1000, 1500})}
        };

        [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void MadOutlierDetectorTest([NotNull] string testDataKey) => Check(TestDataMap[testDataKey]);

        protected override void DumpDetails(TestData testData)
        {
            var values = testData.Values.ToSorted();
            double median = QuantileEstimator.GetMedian(values);
            double mad = MedianAbsoluteDeviation.CalcMad(values, quantileEstimator: QuantileEstimator);

            Output.WriteLine($"Median = {median.ToString(TestCultureInfo.Instance)}");
            Output.WriteLine($"MAD    = {mad.ToString(TestCultureInfo.Instance)}");
        }
    }
}