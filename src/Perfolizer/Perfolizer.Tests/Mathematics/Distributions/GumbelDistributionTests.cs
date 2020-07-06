using Perfolizer.Mathematics.Distributions;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class GumbelDistributionTests : DistributionTestsBase
    {
        public GumbelDistributionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GumbelDistributionTest1()
        {
            var gumbel = new GumbelDistribution();
            AssertEqual("Location", 0, gumbel.Location);
            AssertEqual("Scale", 1, gumbel.Scale);
            AssertEqual("Mean", 0.5772156649015329, gumbel.Mean);
            AssertEqual("Median", 0.36651292058166435, gumbel.Median);
            AssertEqual("Variance", 1.6449340668482264, gumbel.Variance);

            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedPdf = new[]
            {
                0.36610415189774, 0.36105291477093, 0.353165596312007, 0.342898756467732, 0.330704298890418, 0.31701327275429,
                0.302224456630968, 0.286697116378904, 0.270747220321608
            };
            var expectedCdf = new[]
            {
                0.404607661664132, 0.440991025942983, 0.476723690714594, 0.511544833689042, 0.545239211892605, 0.577635844258916,
                0.608605317804406, 0.638056166582019, 0.665930705440122
            };
            var expectedQuantile = new[]
            {
                -0.834032445247956, -0.475884995327111, -0.185626758862366, 0.0874215717907552, 0.366512920581664, 0.671726992092122,
                1.03093043315872, 1.49993998675952, 2.25036732731245
            };

            Check(gumbel, x, expectedPdf, expectedCdf, expectedQuantile);
        }

        [Fact]
        public void GumbelDistributionTest2()
        {
            var gumbel = new GumbelDistribution(1, 2);
            AssertEqual("Location", 1, gumbel.Location);
            AssertEqual("Scale", 2, gumbel.Scale);
            AssertEqual("Mean", 2.1544313298030655, gumbel.Mean);
            AssertEqual("Median", 1.7330258411633288, gumbel.Median);
            AssertEqual("Variance", 6.579736267392906, gumbel.Variance);

            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedPdf = new[]
            {
                0.163415479697888, 0.167801779821723, 0.171664252678358, 0.174993580579211, 0.177786373690972, 0.180044733644614,
                0.181775762939958, 0.182991038252879, 0.183706064004954
            };
            var expectedCdf = new[]
            {
                0.2083966205323, 0.224961793549918, 0.241939508585805, 0.259276865990828, 0.276920334099909, 0.294816320729158,
                0.312911698166345, 0.331154277152909, 0.349493227081447
            };
            var expectedQuantile = new[]
            {
                -0.668064890495911, 0.0482300093457789, 0.628746482275269, 1.17484314358151, 1.73302584116333, 2.34345398418424,
                3.06186086631745, 3.99987997351903, 5.50073465462489
            };

            Check(gumbel, x, expectedPdf, expectedCdf, expectedQuantile);
        }
    }
}