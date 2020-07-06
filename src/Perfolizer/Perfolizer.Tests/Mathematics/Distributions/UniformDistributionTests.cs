using System;
using Perfolizer.Mathematics.Distributions;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class UniformDistributionTests : DistributionTestsBase
    {
        public UniformDistributionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UniformDistributionTest1()
        {
            var uniform = new UniformDistribution(0, 1);
            AssertEqual("Min", 0, uniform.Min);
            AssertEqual("Max", 1, uniform.Max);
            AssertEqual("Mean", 0.5, uniform.Mean);
            AssertEqual("Median", 0.5, uniform.Median);
            AssertEqual("Variance", 1.0 / 12, uniform.Variance);

            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedPdf = new[] {1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0};
            var expectedCdf = x;
            var expectedQuantile = x;

            Check(uniform, x, expectedPdf, expectedCdf, expectedQuantile);
        }

        [Fact]
        public void UniformDistributionTest2()
        {
            var uniform = new UniformDistribution(0.5, 3);
            AssertEqual("Min", 0.5, uniform.Min);
            AssertEqual("Max", 3, uniform.Max);
            AssertEqual("Mean", 1.75, uniform.Mean);
            AssertEqual("Median", 1.75, uniform.Median);
            AssertEqual("Variance", 1.0 / 12 * 2.5 * 2.5, uniform.Variance);

            var x = new[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            var expectedPdf = new[] {0.0, 0.0, 0.0, 0.0, 0.4, 0.4, 0.4, 0.4, 0.4};
            var expectedCdf = new[] {0.0, 0.0, 0.0, 0.0, 0.0, 0.04, 0.08, 0.12, 0.16};
            var expectedQuantile = new[] {0.75, 1, 1.25, 1.5, 1.75, 2, 2.25, 2.5, 2.75};

            Check(uniform, x, expectedPdf, expectedCdf, expectedQuantile);
        }

        [Fact]
        public void UniformDistributionInvalidTest1()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new UniformDistribution(2, 1));
        }
    }
}