using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.SimulationTests.Cpd.TestDataSets;

public static class CpdBinomialMeanProgressionDataSet
{
    private static CpdTestData GenerateSingle(Random random, int count, int meanFactor, int stdDev, int batch, int noise,
        string namePostfix = "")
    {
        string name = $"BinomialMeanProgression(count={count}, meanFactor={meanFactor}, stdDev={stdDev}, batch={batch}){namePostfix}";

        var shuffler = new Shuffler(random);
        int firstModeBatch = batch * 30 / 100;
        int secondModeBatch = batch - firstModeBatch;

        var values = new List<double>();
        for (int i = 0; i < count; i++)
        {
            values.AddRange(new NormalDistribution(mean: 0, stdDev: stdDev).Random(random).Next(firstModeBatch));
            values.AddRange(new NormalDistribution(mean: (i + 1) * meanFactor, stdDev: stdDev).Random(random).Next(secondModeBatch));
            shuffler.Shuffle(values, values.Count - batch, batch);
        }

        var expected = new List<CpdTestData.ExpectedChangePoint>();
        for (int i = 0; i < count - 1; i++)
            expected.Add(new CpdTestData.ExpectedChangePoint((i + 1) * batch - 1, noise, batch / 4));

        return new CpdTestData(name, values, expected, CpdTestData.PenaltyValues.Light, 1);
    }
        
    // ReturnTypeCanBeEnumerable.Global
    public static List<CpdTestData> Generate(Random random, string namePostfix = "")
    {
        var dataSet = new List<CpdTestData>();

        var counts = new[] {2, 10};
        var meanFactors = new[] {20};
        var stdDevToNoise = new Dictionary<int, int>
        {
            {1, 7},
            {5, 10}
        };
        foreach (int count in counts)
        foreach (int meanFactor in meanFactors)
        foreach ((int stdDev, int noise) in stdDevToNoise)
        {
            dataSet.Add(GenerateSingle(random, count, meanFactor, stdDev, 100, noise, namePostfix));
            dataSet.Add(GenerateSingle(random, count, -meanFactor, stdDev, 100, noise, namePostfix));
        }

        return dataSet;
    }
}