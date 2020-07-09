using System;
using System.Collections.Generic;

namespace Perfolizer.Tests.Mathematics.Cpd.TestDataSets
{
    public static class CpdReferenceDataSet
    {
        public static List<CpdTestData> Generate(Random random, int repetitions = 10)
        {
            string GetNamePostfix(int i) => repetitions == 1 ? "" : $"@{i}";

            var dataSet = new List<CpdTestData>();

            dataSet.AddRange(CpdRealDataSet.Generate());

            for (int i = 0; i < repetitions; i++)
            {
                dataSet.AddRange(CpdGaussianMeanProgressionDataSet.Generate(random, GetNamePostfix(i)));
                dataSet.AddRange(CpdBinomialMeanProgressionDataSet.Generate(random, GetNamePostfix(i)));
                dataSet.AddRange(CpdGumbelLocationProgressionDataSet.Generate(random, GetNamePostfix(i)));
                dataSet.AddRange(CpdFrechetLocationProgressionDataSet.Generate(random, GetNamePostfix(i)));
            }

            return dataSet;
        }
    }
}