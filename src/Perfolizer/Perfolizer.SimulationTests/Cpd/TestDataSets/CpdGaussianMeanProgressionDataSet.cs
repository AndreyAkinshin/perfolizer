using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.SimulationTests.Cpd.TestDataSets;

public static class CpdGaussianMeanProgressionDataSet
{
    private static CpdTestData GenerateSingle(Rng rng, int count, int meanFactor, int stdDev, int batch, int noise,
        string namePostfix = "")
    {
        string name = $"GaussianMeanProgression(count={count}, meanFactor={meanFactor}, stdDev={stdDev}, batch={batch}){namePostfix}";

        var values = new List<double>();
        for (int i = 0; i < count; i++)
            values.AddRange(new NormalDistribution(mean: meanFactor * i, stdDev: stdDev).Random(rng).Next(batch));

        var expected = new List<CpdTestData.ExpectedChangePoint>();
        for (int i = 0; i < count - 1; i++)
            expected.Add(new CpdTestData.ExpectedChangePoint((i + 1) * batch - 1, noise, batch / 4));

        return new CpdTestData(name, values, expected);
    }

    public static List<CpdTestData> Generate(Rng rng, string namePostfix = "")
    {
        var dataSet = new List<CpdTestData>();

        var counts = new[] { 2, 3, 4, 10 };
        var meanFactors = new[] { 20 };
        var stdDevToNoise = new Dictionary<int, int>
        {
            {1, 0},
            {5, 1},
            {7, 14}
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