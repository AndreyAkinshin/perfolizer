using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Mathematics.OutlierDetection;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.OutlierDetection
{
    public class DoubleMadOutlierDetectorTests : OutlierDetectorTests
    {
        public DoubleMadOutlierDetectorTests([NotNull] ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        /// Data cases for SimpleQuantileEstimator
        /// </summary>
        private static readonly IDictionary<string, TestData> SimpleQeTestDataMap = new Dictionary<string, TestData>
        {
            {"Empty", new TestData(CornerCaseDataSet.Empty, CornerCaseDataSet.Empty)},
            {"Same", new TestData(CornerCaseDataSet.Same, CornerCaseDataSet.Empty)},
            {"Case1", new TestData(new double[] {1, 4, 4, 4, 5, 5, 5, 5, 7, 7, 8, 10, 16, 30}, new double[] {1, 16, 30})},
            {"Real0", new TestData(RealDataSet.X0, new double[] {38594, 39075})},
            {"Real1", new TestData(RealDataSet.X1, new double[] {0, 0, 0, 0, 1821})},
            {"Real2", new TestData(RealDataSet.X2, new double[] {95, 4364})},
            {"Real3", new TestData(RealDataSet.X3, new double[] {1067, 1085, 1133, 1643, 4642})},
            {"Real4", new TestData(RealDataSet.X4, new double[] { })},
            {"Beta0", new TestData(BetaDataSet.X0, new double[] { })},
            {"Beta1", new TestData(BetaDataSet.X1, new double[] {3071})},
            {"Beta2", new TestData(BetaDataSet.X2, new double[] {3642})},
            {"MBeta_Lower1", new TestData(ModifiedBetaDataSet.Lower1, new double[] {-2000, 3612})},
            {"MBeta_Lower2", new TestData(ModifiedBetaDataSet.Lower2, new double[] {-2001, -2000, 3612})},
            {"MBeta_Lower3", new TestData(ModifiedBetaDataSet.Lower3, new double[] {-2002, -2001, -2000, 3612})},
            {"MBeta_Upper1", new TestData(ModifiedBetaDataSet.Upper1, new double[] {3612, 6000})},
            {"MBeta_Upper2", new TestData(ModifiedBetaDataSet.Upper2, new double[] {6000, 6001})},
            {"MBeta_Upper3", new TestData(ModifiedBetaDataSet.Upper3, new double[] {6000, 6001, 6002})},
            {"MBeta_Both0", new TestData(ModifiedBetaDataSet.Both0, new double[] {-2000, 6000})},
            {"MBeta_Both1", new TestData(ModifiedBetaDataSet.Both1, new double[] {-2001, -2000, 6000, 6001})},
            {"MBeta_Both2", new TestData(ModifiedBetaDataSet.Both2, new double[] {-2002, -2001, -2000, 6000, 6001, 6002})}
        };

        [UsedImplicitly] public static TheoryData<string> SimpleQeTestDataKeys = TheoryDataHelper.Create(SimpleQeTestDataMap.Keys);

        [Theory]
        [MemberData(nameof(SimpleQeTestDataKeys))]
        public void DoubleMadOutlierDetectorSimpleQeTest([NotNull] string testDataKey) => Check(SimpleQeTestDataMap[testDataKey],
            values => DoubleMadOutlierDetector.Create(values, quantileEstimator: SimpleQuantileEstimator.Instance));

        /// <summary>
        /// Data cases for HarrellDavisQuantileEstimator
        /// </summary>
        private static readonly IDictionary<string, TestData> HdQeTestDataMap = new Dictionary<string, TestData>
        {
            {"Empty", new TestData(CornerCaseDataSet.Empty, CornerCaseDataSet.Empty)},
            {"Same", new TestData(CornerCaseDataSet.Same, CornerCaseDataSet.Empty)},
            {"Real0", new TestData(RealDataSet.X0, new double[] {38594, 39075})},
            {"Real1", new TestData(RealDataSet.X1, new double[] {0, 0, 0, 0, 1821})},
            {"Real2", new TestData(RealDataSet.X2, new double[] {95, 4364})},
            {"Real3", new TestData(RealDataSet.X3, new double[] {1067, 1085, 1133, 1643, 4642})},
            {"Real4", new TestData(RealDataSet.X4, new double[] { })},
            {"Beta0", new TestData(BetaDataSet.X0, new double[] { })},
            {"Beta1", new TestData(BetaDataSet.X1, new double[] {3071})},
            {"Beta2", new TestData(BetaDataSet.X2, new double[] {3642})},
            {"MBeta_Lower1", new TestData(ModifiedBetaDataSet.Lower1, new double[] {-2000})},
            {"MBeta_Lower2", new TestData(ModifiedBetaDataSet.Lower2, new double[] {-2001, -2000})},
            {"MBeta_Lower3", new TestData(ModifiedBetaDataSet.Lower3, new double[] {-2002, -2001, -2000})},
            {"MBeta_Upper1", new TestData(ModifiedBetaDataSet.Upper1, new double[] {6000})},
            {"MBeta_Upper2", new TestData(ModifiedBetaDataSet.Upper2, new double[] {6000, 6001})},
            {"MBeta_Upper3", new TestData(ModifiedBetaDataSet.Upper3, new double[] {6000, 6001, 6002})},
            {"MBeta_Both0", new TestData(ModifiedBetaDataSet.Both0, new double[] {-2000, 6000})},
            {"MBeta_Both1", new TestData(ModifiedBetaDataSet.Both1, new double[] {-2001, -2000, 6000, 6001})},
            {"MBeta_Both2", new TestData(ModifiedBetaDataSet.Both2, new double[] {-2002, -2001, -2000, 6000, 6001, 6002})}
        };

        [UsedImplicitly] public static TheoryData<string> HdQeTestDataKeys = TheoryDataHelper.Create(HdQeTestDataMap.Keys);

        [Theory]
        [MemberData(nameof(HdQeTestDataKeys))]
        public void DoubleMadOutlierDetectorHdQeTest([NotNull] string testDataKey) => Check(HdQeTestDataMap[testDataKey],
            values => DoubleMadOutlierDetector.Create(values, quantileEstimator: HarrellDavisQuantileEstimator.Instance));

        [Theory]
        [InlineData(0, 0, 0, "[7; 13] + [27; 33]")]
        [InlineData(1, 1, 1, "{1} + [7; 13] + {19} + [27; 33] + {38}")]
        [InlineData(2, 1, 2, "{1, 2} + [7; 13] + {19} + [27; 33] + {38, 39}")]
        [InlineData(2, 2, 2, "{1, 2} + [7; 13] + {19, 21} + [27; 33] + {38, 39}")]
        public void DoubleMadOutlierDetectorLowlandMultimodalTest(int outlier1, int outlier2, int outlier3, [NotNull] string expected)
        {
            var random = new Random(42);
            var values = new List<double>();

            void AddOutliers(int count, int min, int max)
            {
                if (count >= 1)
                    values.Add(min);
                if (count >= 2)
                    values.Add(max);
                if (count >= 3)
                    values.AddRange(new UniformDistribution(min, max).Random(random).Next(count - 2));
            }

            AddOutliers(outlier1, 1, 2);

            values.Add(7);
            values.AddRange(new NormalDistribution(10, 1).Random(random).Next(100).Clamp(7, 13));
            values.Add(13);

            AddOutliers(outlier2, 19, 21);

            values.Add(27);
            values.AddRange(new NormalDistribution(30, 1).Random(random).Next(100).Clamp(27, 33));
            values.Add(33);

            AddOutliers(outlier3, 38, 39);

            DoubleMadOutlierDetectorMultimodalCheck(values, LowlandModalityDetector.Instance, expected);
        }

        [AssertionMethod]
        private void DoubleMadOutlierDetectorMultimodalCheck([NotNull] IReadOnlyList<double> values,
            [NotNull] IModalityDetector modalityDetector, [NotNull] string expectedSummary)
        {
            var modalityData = modalityDetector.DetectModes(values);
            string actualSummary = modalityData.PresentSummary(OutlierDetectorFactory.DoubleMad, "N0", TestCultureInfo.Instance);
            Output.WriteLine($"Expected : {expectedSummary}");
            Output.WriteLine($"Actual   : {actualSummary}");
            Assert.Equal(expectedSummary, actualSummary);
        }
    }
}