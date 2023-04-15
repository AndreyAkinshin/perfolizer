using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.OutlierDetection;
using Perfolizer.Mathematics.ScaleEstimators;
using Perfolizer.Tests.Common;

namespace Perfolizer.Tests.Mathematics.OutlierDetection;

public class MadOutlierDetectorTests : OutlierDetectorTests
{
    public MadOutlierDetectorTests(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Data cases for SimpleQuantileEstimator
    /// </summary>
    private static readonly IDictionary<string, TestData> SimpleQeTestDataMap = new Dictionary<string, TestData>
    {
        { "Empty", new TestData(CornerCaseDataSet.Empty, CornerCaseDataSet.Empty) },
        { "Same", new TestData(CornerCaseDataSet.Same, CornerCaseDataSet.Empty) },
        { "Yang_X0", new TestData(YangDataSet.X0, new double[] { }) },
        { "Yang_X1", new TestData(YangDataSet.X1, new double[] { 1000 }) },
        { "Yang_X2", new TestData(YangDataSet.X2, new double[] { 500, 1000 }) },
        { "Yang_X3", new TestData(YangDataSet.X3, new double[] { 1000 }) },
        { "Yang_X4", new TestData(YangDataSet.X4, new double[] { 300, 500, 1000, 1500 }) },
        { "Real0", new TestData(RealDataSet.X0, new double[] { 38594, 39075 }) },
        { "Real1", new TestData(RealDataSet.X1, new double[] { 0, 0, 0, 0, 1821 }) },
        { "Real2", new TestData(RealDataSet.X2, new double[] { 95, 4364 }) },
        { "Real3", new TestData(RealDataSet.X3, new double[] { 1067, 1085, 1133, 1643, 4642 }) },
        { "Real4", new TestData(RealDataSet.X4, new double[] { 14893, 15056, 15364, 15483, 16504, 17208, 17446 }) },
        { "Beta0", new TestData(BetaDataSet.X0, new double[] { 2308, 2504, 2556, 2569, 2591, 2604, 2754, 2899, 3262, 3325, 3329 }) },
        { "Beta1", new TestData(BetaDataSet.X1, new double[] { 3071 }) },
        { "Beta2", new TestData(BetaDataSet.X2, new double[] { 3642 }) },
        { "MBeta_Lower1", new TestData(ModifiedBetaDataSet.Lower1, new double[] { -2000, 2919, 3612 }) },
        { "MBeta_Lower2", new TestData(ModifiedBetaDataSet.Lower2, new double[] { -2001, -2000, 2919, 3612 }) },
        { "MBeta_Lower3", new TestData(ModifiedBetaDataSet.Lower3, new double[] { -2002, -2001, -2000, 2919, 3612 }) },
        { "MBeta_Upper1", new TestData(ModifiedBetaDataSet.Upper1, new double[] { 2919, 3612, 6000 }) },
        { "MBeta_Upper2", new TestData(ModifiedBetaDataSet.Upper2, new double[] { 2919, 3612, 6000, 6001 }) },
        { "MBeta_Upper3", new TestData(ModifiedBetaDataSet.Upper3, new double[] { 2919, 3612, 6000, 6001, 6002 }) },
        { "MBeta_Both0", new TestData(ModifiedBetaDataSet.Both0, new double[] { -2000, 2919, 3612, 6000 }) },
        { "MBeta_Both1", new TestData(ModifiedBetaDataSet.Both1, new double[] { -2001, -2000, 2919, 3612, 6000, 6001 }) },
        { "MBeta_Both2", new TestData(ModifiedBetaDataSet.Both2, new double[] { -2002, -2001, -2000, 2919, 3612, 6000, 6001, 6002 }) }
    };

    [UsedImplicitly] public static TheoryData<string> SimpleQeTestDataKeys = TheoryDataHelper.Create(SimpleQeTestDataMap.Keys);

    [Theory]
    [MemberData(nameof(SimpleQeTestDataKeys))]
    public void MadOutlierDetectorSimpleQeTest(string testDataKey)
    {
        var testData = SimpleQeTestDataMap[testDataKey];

        void Action() => Check(testData,
            values => MadOutlierDetector.Create(values, MedianAbsoluteDeviationEstimator.Simple));

        if (testData.Values.IsEmpty())
            Assert.Throws<ArgumentOutOfRangeException>(Action);
        else
            Action();
    }

    /// <summary>
    /// Data cases for HarrellDavisQuantileEstimator
    /// </summary>
    private static readonly IDictionary<string, TestData> HdQeTestDataMap = new Dictionary<string, TestData>
    {
        { "Empty", new TestData(CornerCaseDataSet.Empty, CornerCaseDataSet.Empty) },
        { "Same", new TestData(CornerCaseDataSet.Same, CornerCaseDataSet.Empty) },
        { "Real0", new TestData(RealDataSet.X0, new double[] { 38594, 39075 }) },
        { "Real1", new TestData(RealDataSet.X1, new double[] { 0, 0, 0, 0, 1821 }) },
        { "Real2", new TestData(RealDataSet.X2, new double[] { 95, 4364 }) },
        { "Real3", new TestData(RealDataSet.X3, new double[] { 1067, 1085, 1133, 1643, 4642 }) },
        { "Real4", new TestData(RealDataSet.X4, new double[] { 14893, 15056, 15364, 15483, 16504, 17208, 17446 }) },
        { "Beta0", new TestData(BetaDataSet.X0, new double[] { 2504, 2556, 2569, 2591, 2604, 2754, 2899, 3262, 3325, 3329 }) },
        { "Beta1", new TestData(BetaDataSet.X1, new double[] { 3071 }) },
        { "Beta2", new TestData(BetaDataSet.X2, new double[] { 3642 }) },
        { "MBeta_Lower1", new TestData(ModifiedBetaDataSet.Lower1, new double[] { -2000, 2919, 3612 }) },
        { "MBeta_Lower2", new TestData(ModifiedBetaDataSet.Lower2, new double[] { -2001, -2000, 2919, 3612 }) },
        { "MBeta_Lower3", new TestData(ModifiedBetaDataSet.Lower3, new double[] { -2002, -2001, -2000, 2919, 3612 }) },
        { "MBeta_Upper1", new TestData(ModifiedBetaDataSet.Upper1, new double[] { 2919, 3612, 6000 }) },
        { "MBeta_Upper2", new TestData(ModifiedBetaDataSet.Upper2, new double[] { 2919, 3612, 6000, 6001 }) },
        { "MBeta_Upper3", new TestData(ModifiedBetaDataSet.Upper3, new double[] { 2919, 3612, 6000, 6001, 6002 }) },
        { "MBeta_Both0", new TestData(ModifiedBetaDataSet.Both0, new double[] { -2000, 2919, 3612, 6000 }) },
        { "MBeta_Both1", new TestData(ModifiedBetaDataSet.Both1, new double[] { -2001, -2000, 2919, 3612, 6000, 6001 }) },
        { "MBeta_Both2", new TestData(ModifiedBetaDataSet.Both2, new double[] { -2002, -2001, -2000, 2919, 3612, 6000, 6001, 6002 }) }
    };

    [UsedImplicitly] public static TheoryData<string> HdQeTestDataKeys = TheoryDataHelper.Create(HdQeTestDataMap.Keys);

    [Theory]
    [MemberData(nameof(HdQeTestDataKeys))]
    public void MadOutlierDetectorHdQeTest(string testDataKey)
    {
        var testData = HdQeTestDataMap[testDataKey];

        void Action() => Check(testData,
            values => MadOutlierDetector.Create(values, MedianAbsoluteDeviationEstimator.HarrellDavis));

        if (testData.Values.IsEmpty())
            Assert.Throws<ArgumentOutOfRangeException>(Action);
        else
            Action();
    }
}