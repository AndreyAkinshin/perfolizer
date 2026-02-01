using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Pragmastat.Randomization;

namespace Perfolizer.Tests.Mathematics.Modality.TestDataSets;

public static class ModalityGumbelLocationProgressionDataSet
{
    private static ModalityTestData GenerateSingle(Rng rng, int count, int locationFactor, double scale, int batch,
        string namePostfix = "", bool noisy = false)
    {
        string noisyMark = noisy ? "Noisy" : "";
        string name =
            $"GumbelLocationProgression{noisyMark}(count={count}, locationFactor={locationFactor}, scale={scale}, batch={batch}){namePostfix}";

        var values = new List<double>();
        for (int i = 0; i < count; i++)
        {
            values.AddRange(new GumbelDistribution(location: locationFactor * i, scale: scale).Random(rng).Next(batch));
            if (noisy)
            {
                double d = locationFactor / 5.0;
                values.AddRange(new UniformDistribution(0, 3 * d).Random(rng).Next(batch / 10));
                values.AddRange(new UniformDistribution(-2 * d, 0).Random(rng).Next(batch / 10));
            }
        }

        return new ModalityTestData(name, values, count);
    }

    public static List<ModalityTestData> Generate(Rng rng, string namePostfix = "", bool noisy = false)
    {
        var dataSet = new List<ModalityTestData>();

        int maxCount = noisy ? 8 : 10;
        for (int count = 1; count <= maxCount; count++)
        {
            int batch = 100;
            if (noisy)
                batch += (int)rng.UniformInt(-15, 16);
            dataSet.Add(GenerateSingle(rng, count, 10, 1, batch, namePostfix, noisy));
        }

        return dataSet;
    }
}