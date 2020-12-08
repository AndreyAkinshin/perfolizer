using Perfolizer.Mathematics.Distributions;
using Xunit;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class StudentDistributionTests
    {
        [Theory]
        [InlineData(-2, 3, 0.0696629842794216)]
        [InlineData(-1, 3, 0.195501109477885)]
        [InlineData(-0.5, 3, 0.325723982424076)]
        [InlineData(0, 3, 0.5)]
        [InlineData(0.5, 3, 0.674276017575924)]
        [InlineData(1, 3, 0.804498890522115)]
        [InlineData(2, 3, 0.930337015720578)]
        [InlineData(4.13346570549486, 2, 0.973077323749692)]
        [InlineData(4.13346570549486, 2.088, 0.975)]
        public void Cdf(double x, double df, double expected)
        {
            double actual = new StudentDistribution(df).Cdf(x);
            Assert.Equal(expected, actual, 5);
        }

        [Theory]
        [InlineData(0.1, 3, -1.63774435369621)]
        [InlineData(0.25, 3, -0.764892328404345)]
        [InlineData(0.5, 3, 0)]
        [InlineData(0.75, 3, 0.764892328404345)]
        [InlineData(0.9, 3, 1.63774435369621)]
        [InlineData(0.975, 3, 3.18244630528371)]
        [InlineData(0.975, 2, 4.30265272974946)]
        [InlineData(0.975, 2.088, 4.13346570549486)]
        public void Quantile(double x, double df, double expected)
        {
            double actual = new StudentDistribution(df).Quantile(x);
            Assert.Equal(expected, actual, 5);
        }
        
        [Theory]
        [InlineData(-2, 3, 0.0675096606638929)]
        [InlineData(-1, 3, 0.206748335783172)]
        [InlineData(0, 3, 0.367552596947861)]
        [InlineData(1, 3, 0.206748335783172)]
        [InlineData(2, 3, 0.0675096606638929)]
        [InlineData(2, 3.1, 0.0673978108812023)]
        [InlineData(2, 32, 0.0566892970024942)]
        public void Pdf(double x, double df, double expected)
        {
            double actual = new StudentDistribution(df).Pdf(x);
            Assert.Equal(expected, actual, 5);
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
            double actual = new StudentDistribution(n).Cdf(t);
            Assert.Equal(expected, actual, 4);
        }
    }
}