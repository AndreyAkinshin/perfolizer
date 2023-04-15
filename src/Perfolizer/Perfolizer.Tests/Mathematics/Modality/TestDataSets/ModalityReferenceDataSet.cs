using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Tests.Mathematics.Modality.TestDataSets;

public static class ModalityReferenceDataSet
{
    public static List<ModalityTestData> Generate(Random random, int repetitions = 10)
    {
        string GetNamePostfix(int i) => repetitions == 1 ? "" : $"@{i}";

        var dataSet = new List<ModalityTestData>();

        dataSet.AddRange(ModalityManualDataSet.All);
        for (int i = 0; i < repetitions; i++) 
            dataSet.AddRange(ModalityGumbelLocationProgressionDataSet.Generate(random, GetNamePostfix(i)));
        for (int i = 0; i < repetitions; i++) 
            dataSet.AddRange(ModalityGumbelLocationProgressionDataSet.Generate(random, GetNamePostfix(i), true));
        for (int i = 0; i < repetitions; i++) 
            dataSet.AddRange(ModalityCloseModesDataSet.Generate(random, GetNamePostfix(i)));

        return dataSet;
    }
}