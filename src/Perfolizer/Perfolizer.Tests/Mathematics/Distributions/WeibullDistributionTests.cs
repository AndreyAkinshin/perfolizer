using Perfolizer.Mathematics.Distributions;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class WeibullDistributionTests : DistributionTestsBase
    {
        public WeibullDistributionTests(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void WeibullDistributionTest1()
        {
            var weibull = new WeibullDistribution(1);
            AssertEqual("Shape", 1, weibull.Shape);
            AssertEqual("Scale", 1, weibull.Scale);
            AssertEqual("Mean", 1, weibull.Mean);
            AssertEqual("Median", 0.6931471805599453, weibull.Median);
            AssertEqual("Variance", 1, weibull.Variance);
            AssertEqual("StandardDeviation", 1, weibull.StandardDeviation);

            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedPdf = new[]
            {
                0.90483741803596, 0.818730753077982, 0.740818220681718, 0.670320046035639, 0.606530659712633, 0.548811636094026,
                0.496585303791409, 0.449328964117222, 0.406569659740599
            };
            var expectedCdf = new[]
            {
                0.0951625819640404, 0.181269246922018, 0.259181779318282, 0.329679953964361, 0.393469340287367, 0.451188363905974,
                0.503414696208591, 0.550671035882778, 0.593430340259401
            };
            var expectedQuantile = new[]
            {
                0.105360515657826, 0.22314355131421, 0.356674943938732, 0.510825623765991, 0.693147180559945, 0.916290731874155,
                1.20397280432594, 1.6094379124341, 2.30258509299405
            };

            Check(weibull, x, expectedPdf, expectedCdf, expectedQuantile);
        }

        [Fact]
        public void WeibullDistributionTest2()
        {
            var weibull = new WeibullDistribution(2, 1.5);
            AssertEqual("Shape", 2, weibull.Shape);
            AssertEqual("Scale", 1.5, weibull.Scale);
            AssertEqual("Mean", 1.3293403902848553, weibull.Mean);
            AssertEqual("Median", 1.2488319167365465, weibull.Median);
            AssertEqual("Variance", 0.482854144940259, weibull.Variance);
            AssertEqual("StandardDeviation", 0.694877071819368, weibull.StandardDeviation);

            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedPdf = new[]
            {
                0.0884947037762749, 0.174645211487676, 0.256210517107286, 0.331149654084036, 0.397706363028609, 0.454476687448646,
                0.500455919329689, 0.535062866552393, 0.558141060856825
            };
            var expectedCdf = new[]
            {
                0.00443458251690719, 0.0176206853818224, 0.0392105608476768, 0.068641597888648, 0.10516068318563, 0.147856211033789,
                0.195695843934428, 0.247567843910697, 0.302323673928969
            };
            var expectedQuantile = new[]
            {
                0.486889268961752, 0.708571090616158, 0.895834038124333, 1.07208099203068, 1.24883191673655, 1.43584614312149,
                1.64588541816657, 1.90295436176928, 2.27614069407772
            };

            Check(weibull, x, expectedPdf, expectedCdf, expectedQuantile);
        }
    }
}