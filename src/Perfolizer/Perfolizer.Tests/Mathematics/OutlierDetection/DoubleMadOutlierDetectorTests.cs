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
    public class DoubleMadOutlierDetectorTests : OutlierDetectorTests
    {
        private static readonly IQuantileEstimator QuantileEstimator = SimpleQuantileEstimator.Instance;

        public DoubleMadOutlierDetectorTests([NotNull] ITestOutputHelper output) : base(output)
        {
        }

        protected override IOutlierDetector CreateOutlierDetector(IReadOnlyList<double> values) => DoubleMadOutlierDetector.Create(values,
            quantileEstimator: QuantileEstimator);

        private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
        {
            {"Case1", new TestData(new double[] {1, 4, 4, 4, 5, 5, 5, 5, 7, 7, 8, 10, 16, 30}, new double[] {1, 16, 30})}
        };

        [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void DoubleMadOutlierDetectorTest([NotNull] string testDataKey) => Check(TestDataMap[testDataKey]);

        protected override void DumpDetails(TestData testData)
        {
            var values = testData.Values.ToSorted();
            double median = QuantileEstimator.GetMedian(values);
            double lowerMad = MedianAbsoluteDeviation.CalcLowerMad(values, quantileEstimator: QuantileEstimator);
            double upperMad = MedianAbsoluteDeviation.CalcUpperMad(values, quantileEstimator: QuantileEstimator);

            Output.WriteLine($"Median   = {median.ToString(TestCultureInfo.Instance)}");
            Output.WriteLine($"LowerMAD = {lowerMad.ToString(TestCultureInfo.Instance)}");
            Output.WriteLine($"UpperMAD = {upperMad.ToString(TestCultureInfo.Instance)}");
        }
    }
}