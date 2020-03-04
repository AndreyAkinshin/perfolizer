using Perfolizer.Mathematics.Distributions;
using Xunit;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class NormalDistributionTests
    {
        [Fact]
        public void Cdf()
        {
            var x = new[] {-2, -1, -0.5, 0, 0.5, 1, 2};
            var expectedCdf = new[]
                {0.02275013, 0.15865525, 0.30853754, 0.50000000, 0.69146246, 0.84134475, 0.97724987};
            for (int i = 0; i < x.Length; i++)
            {
                double actualCdf = NormalDistribution.Standard.Cdf(x[i]);
                Assert.Equal(expectedCdf[i], actualCdf, 5);
            }
        }

        [Fact]
        public void Pdf()
        {
            var x = new[] {-2, -1, -0.5, 0, 0.5, 1, 2};
            var expectedCdf = new[]
            {
                0.0539909665131881, 0.241970724519143, 0.3520653267643, 0.398942280401433,
                0.3520653267643, 0.241970724519143, 0.0539909665131881
            };
            for (int i = 0; i < x.Length; i++)
            {
                double actualCdf = NormalDistribution.Standard.Pdf(x[i]);
                Assert.Equal(expectedCdf[i], actualCdf, 5);
            }
        }
    }
}