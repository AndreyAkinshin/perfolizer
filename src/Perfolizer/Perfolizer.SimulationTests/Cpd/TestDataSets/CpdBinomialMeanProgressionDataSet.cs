using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.SimulationTests.Cpd.TestDataSets;

public static class CpdBinomialMeanProgressionDataSet
{
    private static CpdTestData GenerateSingle(Rng rng, int count, int meanFactor, int stdDev, int batch, int noise,
        string namePostfix = "")
    {
        string name = $"BinomialMeanProgression(count={count}, meanFactor={meanFactor}, stdDev={stdDev}, batch={batch}){namePostfix}";

        int firstModeBatch = batch * 30 / 100;
        int secondModeBatch = batch - firstModeBatch;

        var values = new List<double>();
        for (int i = 0; i < count; i++)
        {
            values.AddRange(new NormalDistribution(mean: 0, stdDev: stdDev).Random(rng).Next(firstModeBatch));
            values.AddRange(new NormalDistribution(mean: (i + 1) * meanFactor, stdDev: stdDev).Random(rng).Next(secondModeBatch));
            // Shuffle only the last batch elements
            int offset = values.Count - batch;
            var shuffled = rng.Shuffle(values.GetRange(offset, batch));
            for (int j = 0; j < batch; j++)
                values[offset + j] = shuffled[j];
        }

        var expected = new List<CpdTestData.ExpectedChangePoint>();
        for (int i = 0; i < count - 1; i++)
            expected.Add(new CpdTestData.ExpectedChangePoint((i + 1) * batch - 1, noise, batch / 4));

        return new CpdTestData(name, values, expected, CpdTestData.PenaltyValues.Light, 1);
    }

    // ReturnTypeCanBeEnumerable.Global
    public static List<CpdTestData> Generate(Rng rng, string namePostfix = "")
    {
        var dataSet = new List<CpdTestData>();

        var counts = new[] { 2, 10 };
        var meanFactors = new[] { 20 };
        var stdDevToNoise = new Dictionary<int, int>
        {
            {1, 7},
            {5, 10}
        };
        foreach (int count in counts)
            foreach (int meanFactor in meanFactors)
                foreach ((int stdDev, int noise) in stdDevToNoise)
                {
                    dataSet.Add(GenerateSingle(rng, count, meanFactor, stdDev, 100, noise, namePostfix));
                    dataSet.Add(GenerateSingle(rng, count, -meanFactor, stdDev, 100, noise, namePostfix));
                }

        return dataSet;
    }
}