namespace Perfolizer.SimulationTests.Cpd.TestDataSets;

public static class CpdReferenceDataSet
{
    public static List<CpdTestData> Generate(Rng rng, int repetitions = 10)
    {
        string GetNamePostfix(int i) => repetitions == 1 ? "" : $"@{i}";

        var dataSet = new List<CpdTestData>();

        dataSet.AddRange(CpdRealDataSet.Generate());

        for (int i = 0; i < repetitions; i++)
        {
            dataSet.AddRange(CpdGaussianMeanProgressionDataSet.Generate(rng, GetNamePostfix(i)));
            dataSet.AddRange(CpdBinomialMeanProgressionDataSet.Generate(rng, GetNamePostfix(i)));
            dataSet.AddRange(CpdGumbelLocationProgressionDataSet.Generate(rng, GetNamePostfix(i)));
            dataSet.AddRange(CpdFrechetLocationProgressionDataSet.Generate(rng, GetNamePostfix(i)));
        }

        return dataSet;
    }
}