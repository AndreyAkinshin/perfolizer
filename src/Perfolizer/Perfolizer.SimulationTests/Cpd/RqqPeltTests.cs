using JetBrains.Annotations;
using Perfolizer.Mathematics.Cpd;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.SimulationTests.Cpd.TestDataSets;
using Perfolizer.Tests.Infra;

namespace Perfolizer.SimulationTests.Cpd;

public class RqqPeltTests(ITestOutputHelper output)
{
    private readonly PeltChangePointDetector detector = RqqPeltChangePointDetector.Instance;

    [AssertionMethod]
    private void Check(double[] data, int minDistance, int[] expectedChangePoints)
    {
        var actualChangePoints = detector.GetChangePointIndexes(data, minDistance);
        output.WriteLine("EXPECTED : " + string.Join(", ", expectedChangePoints));
        output.WriteLine("ACTUAL   : " + string.Join(", ", actualChangePoints));
        output.WriteLine("");

        Assert.Equal(expectedChangePoints, actualChangePoints);
    }

    [Fact]
    public void Tie01() => Check(new double[]
    {
        0, 0, 0, 0, 0, 100, 100, 100, 100
    }, 1, new[] { 4 });

    [Fact]
    public void Tie02() => Check(new double[]
    {
        0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2
    }, 1, new[] { 5, 11 });

    [Fact]
    public void Check_WhenTwoMinDistanceLessThanDataLength_ReturnEmptyArray() => Check(new double[]
    {
        0, 0, 0, 0, 0
    }, 4, new int[0]);

    [Fact]
    [Trait(TraitConstants.Category, TraitConstants.Slow)]
    public void ArithmeticProgression() => Check(Enumerable.Range(1, 500).Select(it => (double)it).ToArray(), 10, new[]
    {
        9, 19, 29, 39, 49, 59, 69, 79, 89, 99, 109, 119, 129, 139, 149, 159, 169, 179, 189, 199, 209, 219, 229, 239,
        249, 259, 269, 279, 289, 299, 309, 319, 329, 339, 349, 359, 369, 379, 389, 399, 409, 419, 429, 439, 449,
        459, 469, 479, 489
    });

    [AssertionMethod]
    private void Check100(int count, int error, int[] indexes)
    {
        for (int i = 0; i < indexes.Length; i++)
            output.WriteLine(indexes[i].ToString());

        Assert.Equal(count - 1, indexes.Length);

        int totalError = 0;
        for (int i = 0; i < count - 1; i++)
            totalError += Math.Abs((i + 1) * 100 - 1 - indexes[i]);
        output.WriteLine("Total Error: " + totalError);

        for (int i = 0; i < count - 1; i++)
        {
            int trueValue = (i + 1) * 100 - 1;
            Assert.True(Math.Abs(trueValue - indexes[i]) <= error);
        }
    }

    // TODO: move this test case to CpdReferenceDataSet
    [Theory]
    [InlineData(4, "1;5")]
    [InlineData(4, "1;5;30")]
    [Trait(TraitConstants.Category, TraitConstants.Slow)]
    public void GaussianStdDevProgression(int error, string stdDevValuesString)
    {
        var rng = new Rng(42);

        var stdDevValues = stdDevValuesString.Split(';').Select(double.Parse).ToArray();
        var data = new List<double>();
        foreach (double stdDev in stdDevValues)
            data.AddRange(new NormalDistribution(mean: 0, stdDev: stdDev).Random(rng).Next(100));

        var indexes = detector.GetChangePointIndexes(data.ToArray(), 20);
        Check100(stdDevValues.Length, error, indexes);
    }

    private static readonly IReadOnlyList<CpdTestData> ReferenceDataSet =
        CpdReferenceDataSet.Generate(new Rng(42), 1);

    [UsedImplicitly]
    public static TheoryData<string> ReferenceDataSetNames =
        TheoryDataHelper.Create(ReferenceDataSet.Select(d => d.Name));

    [Theory]
    [MemberData(nameof(ReferenceDataSetNames))]
    [Trait(TraitConstants.Category, TraitConstants.Slow)]
    public void ReferenceDataSetTest(string name)
    {
        var cpdTestData = ReferenceDataSet.First(d => d.Name == name);
        var indexes = detector.GetChangePointIndexes(cpdTestData.Values.ToArray(), 20);
        var verification = CpdTestDataVerification.Verify(cpdTestData, indexes);
        output.WriteLine(verification.Report);
        output.WriteLine("Max penalty: " + verification.Penalty);
        // Tolerance allows for up to 1 missing point + several extra points for high-noise cases
        int tolerance = CpdTestData.PenaltyValues.Default.MissingPoint + CpdTestData.PenaltyValues.Default.ExtraPoint * 5;
        Assert.True(verification.Penalty <= tolerance);
    }
}