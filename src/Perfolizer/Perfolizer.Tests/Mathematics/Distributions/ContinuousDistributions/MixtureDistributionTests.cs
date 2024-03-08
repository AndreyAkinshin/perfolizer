using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Tests.Common;

namespace Perfolizer.Tests.Mathematics.Distributions.ContinuousDistributions;

public class MixtureDistributionTests
{
    private readonly ITestOutputHelper output;

    public MixtureDistributionTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    private static readonly IEqualityComparer<double> Comparer = new AbsoluteEqualityComparer(1e-7);

    [Fact]
    public void MixtureDistributionUniformTest1()
    {
        var mixture = new MixtureDistribution(new UniformDistribution(0, 1), new UniformDistribution(1, 2));
        Assert.Equal(1, mixture.Mean, Comparer);
        Assert.Equal(1, mixture.Median, Comparer);
        Assert.Equal(1.0 / 3, mixture.Variance, Comparer);
        Assert.Equal(Math.Sqrt(1.0 / 3), mixture.StandardDeviation, Comparer);

        Assert.Equal(0, mixture.Cdf(0), Comparer);
        Assert.Equal(0.25, mixture.Cdf(0.5), Comparer);
        Assert.Equal(0.5, mixture.Cdf(1), Comparer);
        Assert.Equal(0.75, mixture.Cdf(1.5), Comparer);
        Assert.Equal(1, mixture.Cdf(2), Comparer);

        for (double p = 0; p < 1.0; p += 0.01)
            Assert.Equal(p, mixture.Cdf(mixture.Quantile(p)), Comparer);
    }

    [Fact]
    public void MixtureGumbelTest1()
    {
        var mixture = new MixtureDistribution(
            new[] {new GumbelDistribution(1, 2), new GumbelDistribution(3, 4), new GumbelDistribution(5, 6)},
            new[] {0.2, 0.3, 0.5}
        );

        for (double p = 0; p < 1.0; p += 0.01)
            Assert.Equal(p, mixture.Cdf(mixture.Quantile(p)), Comparer);
    }

    [Fact]
    public void MixtureExponentialTest1()
    {
        var exp = new ExponentialDistribution();
        var mixture = new MixtureDistribution(exp, new ShiftedDistribution(exp, 10));

        var comparer = new AbsoluteEqualityComparer(1e-4);
        for (double p = 0; p < 1.0; p += 0.01)
        {
            double q = mixture.Quantile(p);
            output.WriteLine($"{p.ToStringInvariant("N2")}: {q.ToStringInvariant()}");
            Assert.Equal(p, mixture.Cdf(q), comparer);
        }
    }
}