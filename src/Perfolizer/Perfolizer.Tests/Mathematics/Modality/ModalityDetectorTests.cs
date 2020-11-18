using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.Histograms;
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

        private static readonly IReadOnlyList<ModalityTestData> ReferenceDataSet = ModalityReferenceDataSet.Generate(new Random(42), 5);

        [UsedImplicitly]
        public static TheoryData<string> ReferenceDataSetNames = TheoryDataHelper.Create(ReferenceDataSet.Select(d => d.Name));

        [Theory]
        [MemberData(nameof(ReferenceDataSetNames))]
        [Trait(TraitConstants.Category, TraitConstants.Slow)]
        public void ReferenceDataSetTest(string name)
        {
            output.WriteLine($"Case: {name}");
            var modalityTestData = ReferenceDataSet.First(d => d.Name == name);
            int expectedModality = modalityTestData.ExpectedModality;
            var modalityData = detector.DetectModes(modalityTestData.Values.ToSample(), QuantileRespectfulDensityHistogramBuilder.Instance,
                diagnostics: true) as LowlandModalityDiagnosticsData;
            if (modalityData == null)
                throw new Exception($"Can't get {nameof(LowlandModalityDiagnosticsData)} from DetectModes");
            
            int actualModality = modalityData.Modality;

            output.WriteLine("ActualModality   : " + actualModality);
            output.WriteLine("ExpectedModality : " + expectedModality);
            output.WriteLine("-----");
            output.WriteLine("Modes:");
            output.WriteLine(Present(modalityData));
            output.WriteLine("-----");
            
            output.WriteLine(StreamUtils.StreamToString(stream => modalityData.DumpAsCsv(stream)));

            Assert.Equal(expectedModality, actualModality);
        }

        [Fact]
        public void WeightedSampleTest()
        {
            var random = new Random(42);
            var values = new GumbelDistribution().Random(random).Next(30).Concat(
                new GumbelDistribution(10).Random(random).Next(10)).ToList();
            double[] weights = ExponentialDecaySequence.CreateFromHalfLife(10).GenerateReverseArray(40);
            var sample = new Sample(values, weights);

            var simpleModalityData = detector.DetectModes(values);
            output.WriteLine("SimpleModalityData.Modes:");
            output.WriteLine(Present(simpleModalityData));
            output.WriteLine();
            output.WriteLine(simpleModalityData.DensityHistogram.Present());
            output.WriteLine("------------------------------");

            var weightedModalityData = detector.DetectModes(sample);
            output.WriteLine("WeightedModalityData.Modes:");
            output.WriteLine(Present(weightedModalityData));
            output.WriteLine();
            output.WriteLine(weightedModalityData.DensityHistogram.Present());

            Assert.Equal(1, simpleModalityData.Modality);
            Assert.Equal(2, weightedModalityData.Modality);
        }

        private string Present(ModalityData data)
        {
            var formatter = new ManualModalityDataFormatter
            {
                PresentOutliers = false,
                PresentModeLocations = true,
                GroupSeparator = Environment.NewLine
            };
            return formatter.Format(data, "N2");
        }
    }
}