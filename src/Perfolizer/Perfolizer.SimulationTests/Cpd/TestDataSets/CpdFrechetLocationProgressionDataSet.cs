using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.SimulationTests.Cpd.TestDataSets;

public static class CpdFrechetLocationProgressionDataSet
{
    private static CpdTestData GenerateSingle(Rng rng, int count, int locationFactor, double scale, int batch, int noise, string namePostfix = "")
    {
        string name = $"FrechetLocationProgression(count={count}, locationFactor={locationFactor}, scale={scale}, batch={batch}){namePostfix}";

        var values = new List<double>();
        for (int i = 0; i < count; i++)
            values.AddRange(new FrechetDistribution(location: locationFactor * i, scale: scale).Random(rng).Next(batch));

        var expected = new List<CpdTestData.ExpectedChangePoint>();
        for (int i = 0; i < count - 1; i++)
            expected.Add(new CpdTestData.ExpectedChangePoint((i + 1) * batch - 1, noise, batch / 4));

        return new CpdTestData(name, values, expected, CpdTestData.PenaltyValues.Light, 1);
    }

    public static List<CpdTestData> Generate(Rng rng, string namePostfix = "")
    {
        var dataSet = new List<CpdTestData>();

        var counts = new[] { 2, 3, 4, 10 };
        var parameters = new List<(int LocationFactor, double Scale, int Noise)>
        {
            (2, 1, 10),
            (-2, 1, 10),
            (3, 1, 10),
            (-3, 1, 10),
            (5, 1, 10),
            (-5, 1, 10),
            (10, 1, 10),
            (-10, 1, 10),
            (10, 2, 10),
            (-10, 2, 10)
        };

        foreach (int count in counts)
            foreach ((int locationFactor, double scale, int noise) in parameters)
                dataSet.Add(GenerateSingle(rng, count, locationFactor, scale, 100, noise, namePostfix));

        return dataSet;
    }
}