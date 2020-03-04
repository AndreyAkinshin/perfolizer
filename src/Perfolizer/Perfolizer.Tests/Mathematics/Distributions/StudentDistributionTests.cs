using Perfolizer.Mathematics.Distributions;
using Xunit;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class StudentDistributionTests
    {
        [Fact]
        public void Cdf()
        {
            var x = new[] {-2, -1, -0.5, 0, 0.5, 1, 2};
            var expectedCdf = new[]
            {
                0.0696629842794216, 0.195501109477885, 0.325723982424076, 0.5,
                0.674276017575924, 0.804498890522115, 0.930337015720578
            };
            for (int i = 0; i < x.Length; i++)
            {
                double actualCdf = new StudentDistribution(3).Cdf(x[i]);
                Assert.Equal(expectedCdf[i], actualCdf, 5);
            }
        }

        [Fact]
        public void Quantile()
        {
            var x = new[] {0.1, 0.25, 0.5, 0.75, 0.9};
            var expected = new[]
            {
                -1.63774435369621,
                -0.764892328404345, 0, 0.764892328404345,
                1.63774435369621
            };
            for (int i = 0; i < x.Length; i++)
            {
                double actual = new StudentDistribution(3).Quantile(x[i]);
                Assert.Equal(expected[i], actual, 5);
            }
        }
        
        [Theory]
        [InlineData(-1.8084, 507.2, 0.03556814)]
        [InlineData(1.8084, 507.2, 0.9644319)]
        [InlineData(-1.488, 507.2, 0.06868611)]
        [InlineData(1.488, 507.2, 0.9313139)]
        [InlineData(-1.488, 20, 0.07617457)]
        [InlineData(1.488, 20, 0.9238254)]
        public void StudentOneTailTest(double t, double n, double expected)
        {
            double actual = StudentDistribution.StudentOneTail(t, n);
            Assert.Equal(expected, actual, 4);
        }
    }
}