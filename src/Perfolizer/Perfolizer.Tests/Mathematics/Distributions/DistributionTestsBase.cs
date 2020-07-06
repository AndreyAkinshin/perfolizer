using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class DistributionTestsBase
    {
        private static readonly IEqualityComparer<double> EqualityComparer = new AbsoluteEqualityComparer(1e-5);

        private readonly ITestOutputHelper output;

        protected DistributionTestsBase(ITestOutputHelper output)
        {
            this.output = output;
        }

        [AssertionMethod]
        protected void Check([NotNull] IDistribution distribution, [NotNull] double[] x, [NotNull] double[] expectedPdf,
            [NotNull] double[] expectedCdf, [NotNull] double[] expectedQuantile)
        {
            AssertEqual("StandardDeviation", distribution.StandardDeviation, distribution.Variance.Sqrt());

            for (int i = 0; i < x.Length; i++)
                AssertEqual($"Pdf({x[i]})", expectedPdf[i], distribution.Pdf(x[i]));

            for (int i = 0; i < x.Length; i++)
                AssertEqual($"Cdf({x[i]})", expectedCdf[i], distribution.Cdf(x[i]));

            for (int i = 0; i < x.Length; i++)
                AssertEqual($"Quantile({x[i]})", expectedQuantile[i], distribution.Quantile(x[i]));

            for (int i = 0; i < x.Length; i++)
                AssertEqual($"Cdf(Quantile({x[i]}))", x[i], distribution.Cdf(distribution.Quantile(x[i])));

            Assert.Throws<ArgumentOutOfRangeException>(() => distribution.Quantile(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => distribution.Quantile(2));
        }

        [AssertionMethod]
        protected void AssertEqual([NotNull] string name, double expected, double actual)
        {
            output.WriteLine($"{name}:");
            output.WriteLine($"  Expected = {expected}");
            output.WriteLine($"  Actual   = {actual}");
            Assert.Equal(expected, actual, EqualityComparer);
        }
    }
}