using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Tests.Common;
using Xunit;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class NormalDistributionTests
    {
        private class TestData
        {
            public double Mean { get; }
            public double StdDev { get; }
            public double[] X { get; }
            public double[] Expected { get; }

            public TestData(double mean, double stdDev, double[] x, double[] expected)
            {
                Mean = mean;
                StdDev = stdDev;
                X = x;
                Expected = expected;
            }
        }

        private static readonly IDictionary<string, TestData> TestDataCdfMap = new Dictionary<string, TestData>
        {
            {
                "Normal", new TestData(0, 1,
                    new[] {-2, -1, -0.5, 0, 0.5, 1, 2},
                    new[] {0.02275013, 0.15865525, 0.30853754, 0.50000000, 0.69146246, 0.84134475, 0.97724987})
            },
            {
                "Non-Normal", new TestData(0.4, 0.06859943,
                    new[] {0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1},
                    new[]
                    {
                        2.75560362599496e-09, 6.12153240604948e-06, 0.00177573240385303,
                        0.0724563903733593, 0.5, 0.927543609626641, 0.998224267596147,
                        0.999993878467594, 0.999999997244396, 0.999999999999843, 1
                    })
            }
        };

        [UsedImplicitly] public static TheoryData<string> TestDataCdfKeys = TheoryDataHelper.Create(TestDataCdfMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataCdfKeys))]
        public void Cdf(string testDataKey)
        {
            var testData = TestDataCdfMap[testDataKey];
            var distribution = new NormalDistribution(testData.Mean, testData.StdDev);
            var comparer = new AbsoluteEqualityComparer(1e-5);
            var x = testData.X;
            var expectedCdf = testData.Expected;
            for (int i = 0; i < x.Length; i++)
            {
                double actualCdf = distribution.Cdf(x[i]);
                Assert.Equal(expectedCdf[i], actualCdf, comparer);
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

        [Fact]
        public void Quantile()
        {
            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedQuantile = new[]
            {
                -1.2815515655446, -0.841621233572914, -0.524400512708041, -0.2533471031358, 0, 0.2533471031358, 0.524400512708041,
                0.841621233572914, 1.2815515655446
            };
            for (int i = 0; i < x.Length; i++)
            {
                double actualQuantile = NormalDistribution.Standard.Quantile(x[i]);
                Assert.Equal(expectedQuantile[i], actualQuantile, new AbsoluteEqualityComparer(1e-3));
            }
        }
    }
}