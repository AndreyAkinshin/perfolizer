using Pragmastat.Randomization;

namespace Perfolizer.Tests.Mathematics.Modality.TestDataSets;

public static class ModalityReferenceDataSet
{
    public static List<ModalityTestData> Generate(Rng rng, int repetitions = 10)
    {
        string GetNamePostfix(int i) => repetitions == 1 ? "" : $"@{i}";

        var dataSet = new List<ModalityTestData>();

        dataSet.AddRange(ModalityManualDataSet.All);
        for (int i = 0; i < repetitions; i++)
            dataSet.AddRange(ModalityGumbelLocationProgressionDataSet.Generate(rng, GetNamePostfix(i)));
        for (int i = 0; i < repetitions; i++)
            dataSet.AddRange(ModalityGumbelLocationProgressionDataSet.Generate(rng, GetNamePostfix(i), true));
        for (int i = 0; i < repetitions; i++)
            dataSet.AddRange(ModalityCloseModesDataSet.Generate(rng, GetNamePostfix(i)));

        return dataSet;
    }
}