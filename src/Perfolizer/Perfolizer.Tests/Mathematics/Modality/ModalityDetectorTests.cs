using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Mathematics.Sequences;
using Perfolizer.Tests.Common;
using Perfolizer.Tests.Mathematics.Modality.TestDataSets;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Modality
{
    public class ModalityDetectorTests
    {
        private readonly ITestOutputHelper output;
        private readonly LowlandModalityDetector detector = LowlandModalityDetector.Instance;

        public ModalityDetectorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static readonly IReadOnlyList<ModalityTestData> ReferenceDataSet = ModalityReferenceDataSet.Generate(new Random(42), 1);

        [UsedImplicitly]
        public static TheoryData<string> ReferenceDataSetNames = TheoryDataHelper.Create(ReferenceDataSet.Select(d => d.Name));

        [Theory]
        [MemberData(nameof(ReferenceDataSetNames))]
        public void ReferenceDataSetTest(string name)
        {
            var modalityTestData = ReferenceDataSet.First(d => d.Name == name);
            int expectedModality = modalityTestData.ExpectedModality;
            var modalityData = detector.DetectModes(modalityTestData.Values);
            int actualModality = modalityData.Modality;

            output.WriteLine("ActualModality   : " + actualModality);
            output.WriteLine("ExpectedModality : " + expectedModality);
            output.WriteLine("-----");
            output.WriteLine("Modes:");
            output.WriteLine(modalityData.Present());

            Assert.Equal(expectedModality, actualModality);
        }

        [Fact]
        public void WeightedSampleTest()
        {
            var random = new Random(42);
            var values = new GumbelDistribution().Random(random).Next(30).Concat(
                new GumbelDistribution(10).Random(random).Next(10)).ToList();
            var weights = ExponentialDecaySequence.CreateFromHalfLife(10).GenerateReverseArray(40);

            var simpleModalityData = detector.DetectModes(values);
            output.WriteLine("SimpleModalityData.Modes:");
            output.WriteLine(simpleModalityData.Present());
            output.WriteLine();
            output.WriteLine(simpleModalityData.DensityHistogram.Present());
            output.WriteLine("------------------------------");
            
            var weightedModalityData = detector.DetectModes(values, weights);
            output.WriteLine("WeightedModalityData.Modes:");
            output.WriteLine(weightedModalityData.Present());
            output.WriteLine();
            output.WriteLine(weightedModalityData.DensityHistogram.Present());
            
            Assert.Equal(1, simpleModalityData.Modality);
            Assert.Equal(2, weightedModalityData.Modality);
        }
    }
}