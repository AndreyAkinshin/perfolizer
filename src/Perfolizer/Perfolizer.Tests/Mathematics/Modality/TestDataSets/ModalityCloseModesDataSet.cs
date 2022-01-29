using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Tests.Mathematics.Modality.TestDataSets
{
    public static class ModalityCloseModesDataSet
    {
        private static ModalityTestData GenerateSingle(Random random, double delta, int batch, string namePostfix = "")
        {
            string name = $"CloseModes(delta = {delta}, batch={batch}){namePostfix}";

            var values = new List<double>();
            values.AddRange(new UniformDistribution(0, 1).Random(random).Next(batch).Select(x => delta + x * x * x));
            values.AddRange(new UniformDistribution(0, 1).Random(random).Next(batch).Select(x => -delta - x * x * x));

            return new ModalityTestData(name, values, 2);
        }

        public static List<ModalityTestData> Generate(Random random, string namePostfix = "")
        {
            var dataSet = new List<ModalityTestData>();

            const int batchSize = 1000;
            dataSet.Add(GenerateSingle(random, 0.5, batchSize, namePostfix));
            dataSet.Add(GenerateSingle(random, 0.1, batchSize, namePostfix));
            dataSet.Add(GenerateSingle(random, 0.01, batchSize, namePostfix));

            return dataSet;
        }
    }
}