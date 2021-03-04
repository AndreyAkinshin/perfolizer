using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Tests.Mathematics.Modality.TestDataSets
{
    public static class ModalityGumbelLocationProgressionDataSet
    {
        [NotNull]
        private static ModalityTestData GenerateSingle([NotNull] Random random, int count, int locationFactor, double scale, int batch,
            [NotNull] string namePostfix = "", bool noisy = false)
        {
            string noisyMark = noisy ? "Noisy" : "";
            string name =
                $"GumbelLocationProgression{noisyMark}(count={count}, locationFactor={locationFactor}, scale={scale}, batch={batch}){namePostfix}";

            var values = new List<double>();
            for (int i = 0; i < count; i++)
            {
                values.AddRange(new GumbelDistribution(location: locationFactor * i, scale: scale).Random(random).Next(batch));
                if (noisy)
                {
                    double d = locationFactor / 5.0;
                    values.AddRange(new UniformDistribution(0, 3 * d).Random(random).Next(batch / 10));
                    values.AddRange(new UniformDistribution(-2 * d, 0).Random(random).Next(batch / 10));
                }
            }

            return new ModalityTestData(name, values, count);
        }

        public static List<ModalityTestData> Generate([NotNull] Random random, [NotNull] string namePostfix = "", bool noisy = false)
        {
            var dataSet = new List<ModalityTestData>();

            int maxCount = noisy ? 8 : 10;
            for (int count = 1; count <= maxCount; count++)
            {
                int batch = 100;
                if (noisy)
                    batch += random.Next(-15, 15);
                dataSet.Add(GenerateSingle(random, count, 10, 1, batch, namePostfix, noisy));
            }

            return dataSet;
        }
    }
}