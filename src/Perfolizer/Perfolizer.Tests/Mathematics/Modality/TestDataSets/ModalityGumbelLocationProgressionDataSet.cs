using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions;

namespace Perfolizer.Tests.Mathematics.Modality.TestDataSets
{
    public static class ModalityGumbelLocationProgressionDataSet
    {
        [NotNull]
        private static ModalityTestData GenerateSingle([NotNull] Random random, int count, int locationFactor, double scale, int batch,
            [NotNull] string namePostfix = "")
        {
            string name =
                $"GumbelLocationProgression(count={count}, locationFactor={locationFactor}, scale={scale}, batch={batch}){namePostfix}";

            var values = new List<double>();
            for (int i = 0; i < count; i++)
                values.AddRange(new GumbelDistribution(location: locationFactor * i, scale: scale).Random(random).Next(batch));

            return new ModalityTestData(name, values, count);
        }

        public static List<ModalityTestData> Generate([NotNull] Random random, [NotNull] string namePostfix = "")
        {
            var dataSet = new List<ModalityTestData>();

            for (int count = 1; count <= 10; count++)
                dataSet.Add(GenerateSingle(random, count, 10, 1, 100, namePostfix));

            return dataSet;
        }
    }
}