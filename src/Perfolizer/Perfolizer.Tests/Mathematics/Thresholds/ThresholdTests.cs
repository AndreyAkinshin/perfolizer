using Perfolizer.Mathematics.Thresholds;
using System.Globalization;
using Xunit;

namespace Perfolizer.Tests.Mathematics.Thresholds
{
    public class ThresholdTests
    {
        [Theory]
        [InlineData("1.5%", 1.5 / 100.0, ThresholdUnit.Ratio)]
        [InlineData("1.5ns", 1.5, ThresholdUnit.Nanoseconds)]
        [InlineData("1\u03BCs", 1.0, ThresholdUnit.Microseconds)]
        [InlineData("10ms", 10.0, ThresholdUnit.Milliseconds)]
        [InlineData("10,000s", 10_000.0, ThresholdUnit.Seconds)]
        [InlineData("10,000.5m", 10_000.5, ThresholdUnit.Minutes)]
        public void ParseTest(string input, double expectedValue, ThresholdUnit expectedUnit)
        {
            Assert.True(Threshold.TryParse(input, out var parsed));
            Assert.Equal(Threshold.Create(expectedUnit, expectedValue), parsed);
        }

        [Theory]
        [InlineData("1,5%", 1.5 / 100.0, ThresholdUnit.Ratio)]
        [InlineData("1,5ns", 1.5, ThresholdUnit.Nanoseconds)]
        [InlineData("1\u03BCs", 1.0, ThresholdUnit.Microseconds)]
        [InlineData("10ms", 10.0, ThresholdUnit.Milliseconds)]
        [InlineData("10000s", 10_000.0, ThresholdUnit.Seconds)]
        [InlineData("10000,5m", 10_000.5, ThresholdUnit.Minutes)]
        public void ParseTestDifferentFormat(string input, double expectedValue, ThresholdUnit expectedUnit)
        {
            var numberStyle = NumberStyles.Any;
            var frenchFormat = new CultureInfo("fr-FR");

            Assert.True(Threshold.TryParse(input, numberStyle, frenchFormat, out var parsed));
            Assert.Equal(Threshold.Create(expectedUnit, expectedValue), parsed);
        }

        [Fact]
        public void ParseTestInvalidFormat()
        {
            var numberStyle = NumberStyles.Any;
            var frenchFormat = new CultureInfo("fr-FR");

            Assert.False(Threshold.TryParse("1,000.5s", numberStyle, frenchFormat, out _));
            Assert.False(Threshold.TryParse("1.5s", numberStyle, frenchFormat, out _));
        }
    }
}
