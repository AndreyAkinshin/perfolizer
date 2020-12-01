using Perfolizer.Mathematics.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Common
{
    public class ConfidenceIntervalTests
    {
        private readonly ITestOutputHelper output;

        public ConfidenceIntervalTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(0.95, 2, 12.706205)]
        [InlineData(0.95, 3, 4.302653)]
        [InlineData(0.95, 4, 3.182446)]
        [InlineData(0.95, 5, 2.776445)]
        [InlineData(0.95, 6, 2.570582)]
        [InlineData(0.95, 7, 2.446912)]
        [InlineData(0.95, 8, 2.364624)]
        [InlineData(0.95, 9, 2.306004)]
        [InlineData(0.95, 10, 2.262157)]
        [InlineData(0.95, 11, 2.228139)]
        [InlineData(0.95, 12, 2.200985)]
        [InlineData(0.95, 13, 2.178813)]
        [InlineData(0.95, 14, 2.160369)]
        [InlineData(0.95, 15, 2.144787)]
        [InlineData(0.95, 16, 2.131450)]
        [InlineData(0.95, 17, 2.119905)]
        [InlineData(0.95, 18, 2.109816)]
        [InlineData(0.95, 19, 2.100922)]
        [InlineData(0.95, 20, 2.093024)]
        [InlineData(0.95, 100, 1.984217)]
        [InlineData(0.99, 2, 63.656741)]
        [InlineData(0.99, 3, 9.924843)]
        [InlineData(0.99, 4, 5.840909)]
        [InlineData(0.99, 5, 4.604095)]
        [InlineData(0.99, 6, 4.032143)]
        [InlineData(0.99, 7, 3.707428)]
        [InlineData(0.99, 8, 3.499483)]
        [InlineData(0.99, 9, 3.355387)]
        [InlineData(0.99, 10, 3.249836)]
        [InlineData(0.99, 11, 3.169273)]
        [InlineData(0.99, 12, 3.105807)]
        [InlineData(0.99, 13, 3.054540)]
        [InlineData(0.99, 14, 3.012276)]
        [InlineData(0.99, 15, 2.976843)]
        [InlineData(0.99, 16, 2.946713)]
        [InlineData(0.99, 17, 2.920782)]
        [InlineData(0.99, 18, 2.898231)]
        [InlineData(0.99, 19, 2.878440)]
        [InlineData(0.99, 20, 2.860935)]
        [InlineData(0.99, 100, 2.626405)]
        [InlineData(0.999, 2, 636.619249)]
        [InlineData(0.999, 3, 31.599055)]
        [InlineData(0.999, 4, 12.923979)]
        [InlineData(0.999, 5, 8.610302)]
        [InlineData(0.999, 6, 6.868827)]
        [InlineData(0.999, 7, 5.958816)]
        [InlineData(0.999, 8, 5.407883)]
        [InlineData(0.999, 9, 5.041305)]
        [InlineData(0.999, 10, 4.780913)]
        [InlineData(0.999, 11, 4.586894)]
        [InlineData(0.999, 12, 4.436979)]
        [InlineData(0.999, 13, 4.317791)]
        [InlineData(0.999, 14, 4.220832)]
        [InlineData(0.999, 15, 4.140454)]
        [InlineData(0.999, 16, 4.072765)]
        [InlineData(0.999, 17, 4.014996)]
        [InlineData(0.999, 18, 3.965126)]
        [InlineData(0.999, 19, 3.921646)]
        [InlineData(0.999, 20, 3.883406)]
        [InlineData(0.999, 100, 3.391529)]
        public void ZValueTest(double confidenceLevel, int n, double expected)
        {
            var approximation = new ConfidenceIntervalEstimator(n, 0, 1);
            var confidenceInterval = approximation.GetConfidenceInterval(confidenceLevel);
            double actual = confidenceInterval.Upper;
            output.WriteLine(confidenceInterval.ToString());
            
            Assert.Equal(expected, actual, 3);
        }
    }
}