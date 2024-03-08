using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.SimulationTests.Cpd.TestDataSets;

public static class CpdGumbelLocationProgressionDataSet
{
    private static CpdTestData GenerateSingle(Random random, int count, int locationFactor, double scale, int batch, int noise, string namePostfix = "")
    {
        string name = $"GumbelLocationProgression(count={count}, locationFactor={locationFactor}, scale={scale}, batch={batch}){namePostfix}";

        var values = new List<double>();
        for (int i = 0; i < count; i++)
            values.AddRange(new GumbelDistribution(location: locationFactor * i, scale: scale).Random(random).Next(batch));

        var expected = new List<CpdTestData.ExpectedChangePoint>();
        for (int i = 0; i < count - 1; i++)
            expected.Add(new CpdTestData.ExpectedChangePoint((i + 1) * batch - 1, noise, batch / 4));

        var penalties = new CpdTestData.PenaltyValues(
            CpdTestData.PenaltyValues.Default.MissingPoint,
            CpdTestData.PenaltyValues.Default.AcceptableMissingPoint,
            CpdTestData.PenaltyValues.Default.ExtraPoint / 10,
            CpdTestData.PenaltyValues.Default.Displacement);
            
        return new CpdTestData(name, values, expected, penalties);
    }

    public static List<CpdTestData> Generate(Random random, string namePostfix = "")
    {
        var dataSet = new List<CpdTestData>();

        var counts = new[] {2, 3, 4, 10};
        var parameters = new List<(int LocationFactor, double Scale, int Noise)>
        {
            (5, 1, 1),
            (-5, 1, 2),
            (10, 1, 0),
            (-10, 1, 0),
            (10, 2, 1),
            (-10, 2, 1)
        };

        foreach (int count in counts)
        foreach ((int locationFactor, double scale, int noise) in parameters)
            dataSet.Add(GenerateSingle(random, count, locationFactor, scale, 100, noise, namePostfix));

        return dataSet;
    }
}