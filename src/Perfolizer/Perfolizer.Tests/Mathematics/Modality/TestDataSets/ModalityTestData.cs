using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Tests.Mathematics.Modality.TestDataSets
{
    public class ModalityTestData
    {
        public string Name { get; }
        public IReadOnlyList<double> Values { get; }
        public int ExpectedModality { get; }

        public ModalityTestData(string name, IReadOnlyList<double> values, int expectedModality)
        {
            Name = name;
            Values = values;
            ExpectedModality = expectedModality;
        }
    }
}