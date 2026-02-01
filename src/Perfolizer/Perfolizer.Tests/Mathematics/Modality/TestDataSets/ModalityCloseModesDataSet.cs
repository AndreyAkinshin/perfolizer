using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Pragmastat.Randomization;

namespace Perfolizer.Tests.Mathematics.Modality.TestDataSets;

public static class ModalityCloseModesDataSet
{
    private static ModalityTestData GenerateSingle(Rng rng, double delta, int batch, string namePostfix = "")
    {
        string name = $"CloseModes(delta = {delta}, batch={batch}){namePostfix}";

        var values = new List<double>();
        values.AddRange(new UniformDistribution(0, 1).Random(rng).Next(batch).Select(x => delta + x * x * x));
        values.AddRange(new UniformDistribution(0, 1).Random(rng).Next(batch).Select(x => -delta - x * x * x));

        return new ModalityTestData(name, values, 2);
    }

    public static List<ModalityTestData> Generate(Rng rng, string namePostfix = "")
    {
        var dataSet = new List<ModalityTestData>();

        const int batchSize = 1000;
        dataSet.Add(GenerateSingle(rng, 0.5, batchSize, namePostfix));
        dataSet.Add(GenerateSingle(rng, 0.1, batchSize, namePostfix));
        dataSet.Add(GenerateSingle(rng, 0.01, batchSize, namePostfix));

        return dataSet;
    }
}