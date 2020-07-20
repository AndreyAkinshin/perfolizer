using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Tests.Common;
using Perfolizer.Tests.Mathematics.Modality.TestDataSets;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Modality
{
    public class ModalityDetectorTests
    {
        private readonly ITestOutputHelper output;
        private readonly ModalityDetector detector = ModalityDetector.Instance;

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
            for (int i = 0; i < modalityData.Modality; i++)
            {
                double left = i == 0 ? modalityTestData.Values.Min() : modalityData.CutPoints[i - 1];
                double right = i == modalityData.Modality - 1 ? modalityTestData.Values.Max() : modalityData.CutPoints[i];
                double value = modalityData.Modes[i];
                output.WriteLine($"{left:0.00} | {value:0.00} | {right:0.00}");
            }
            
            Assert.Equal(expectedModality, actualModality);
        }
    }
}