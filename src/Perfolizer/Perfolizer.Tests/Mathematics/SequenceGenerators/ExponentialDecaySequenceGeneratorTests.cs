using System.Collections.Generic;
using System.Linq;
using Perfolizer.Mathematics.Sequences;
using Perfolizer.Tests.Common;
using Xunit;

namespace Perfolizer.Tests.Mathematics.SequenceGenerators
{
    public class ExponentialDecaySequenceGeneratorTests
    {
        private readonly IEqualityComparer<double> equalityComparer = new AbsoluteEqualityComparer(1e-9);

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(1, 1, 1.0 / 2)]
        [InlineData(2, 0, 1)]
        [InlineData(2, 2, 1.0 / 2)]
        [InlineData(30, 0, 1)]
        [InlineData(30, 30, 1.0 / 2)]
        [InlineData(30, 60, 1.0 / 4)]
        [InlineData(30, 90, 1.0 / 8)]
        [InlineData(30, 120, 1.0 / 16)]
        public void ExponentialDecayHalfLifeGetTest(int halfLife, int index, double expectedValue)
        {
            var sequence = ExponentialDecaySequence.CreateFromHalfLife(halfLife);
            
            double actualValue1 = sequence.GetValue(index);
            Assert.Equal(expectedValue, actualValue1, equalityComparer);
            
            double actualValue2 = sequence.GenerateEnumerable().Skip(index).First();
            Assert.Equal(expectedValue, actualValue2, equalityComparer);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 1, 2)]
        [InlineData(1, 2, 2)]
        [InlineData(10, 10, 5)]
        [InlineData(10000, 10, 0.1)]
        public void ExponentialDecayNormalizationTest(int count, int initialValue, double decayConstant)
        {
            var values = new ExponentialDecaySequence(initialValue, decayConstant).GenerateArray(count, normalize: true);
            Assert.Equal(1.0, values.Sum(), equalityComparer);
        }
    }
}