using Perfolizer.Mathematics.Distributions;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class FrechetDistributionTests : DistributionTestsBase
    {
        public FrechetDistributionTests(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void FrechetDistributionTest1()
        {
            var frechet = new FrechetDistribution();
            AssertEqual("Location", 0, frechet.Location);
            AssertEqual("Scale", 1, frechet.Scale);
            AssertEqual("Mean", double.PositiveInfinity, frechet.Mean);
            AssertEqual("Median", 1.4426950408889634, frechet.Median);
            AssertEqual("Variance", double.PositiveInfinity, frechet.Variance);

            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedPdf = new[]
            {
                0.00453999297624848, 0.168448674977137, 0.39637770385836, 0.513031241399367, 0.541341132946451, 0.524654452326561,
                0.489083747840359, 0.447663745094047, 0.406411096059143
            };
            var expectedCdf = new[]
            {
                4.53999297624849e-05, 0.00673794699908547, 0.0356739933472524, 0.0820849986238988, 0.135335283236613, 0.188875602837562,
                0.239651036441776, 0.28650479686019, 0.329192987807906
            };
            var expectedQuantile = new[]
            {
                0.434294481903252, 0.621334934559612, 0.830583545082537, 1.09135666793729, 1.44269504088896, 1.95761518897122,
                2.80367325205713, 4.48142011772455, 9.4912215810299
            };

            Check(frechet, x, expectedPdf, expectedCdf, expectedQuantile);
        }

        [Fact]
        public void FrechetDistributionTest2()
        {
            var frechet = new FrechetDistribution(-1, 2, 3);
            AssertEqual("Location", -1, frechet.Location);
            AssertEqual("Scale", 2, frechet.Scale);
            AssertEqual("Shape", 3, frechet.Shape);
            AssertEqual("Mean", 1.7082358814691916, frechet.Mean);
            AssertEqual("Median", 1.2598945526747802, frechet.Median);
            AssertEqual("Variance", 3.3812125776279194, frechet.Variance);

            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedPdf = new[]
            {
                0.0402073567615389, 0.112944127838169, 0.2203072066895, 0.338481103916981, 0.443003781677631, 0.519397555251499,
                0.56394859909252, 0.579937498912823, 0.573663383639088
            };
            var expectedCdf = new[]
            {
                0.00245281629310705, 0.00975837264521781, 0.0262174755427451, 0.0541795420336448, 0.0934461101976254, 0.141830159087343,
                0.196256462270027, 0.253664662024469, 0.311501607580123
            };
            var expectedQuantile = new[]
            {
                0.514577262661815, 0.706626899067589, 0.87999965676962, 1.05913852685906, 1.25989455267478, 1.50191434769605,
                1.82015164486637, 2.29737657865596, 3.23451848624939
            };

            Check(frechet, x, expectedPdf, expectedCdf, expectedQuantile);
        }
    }
}