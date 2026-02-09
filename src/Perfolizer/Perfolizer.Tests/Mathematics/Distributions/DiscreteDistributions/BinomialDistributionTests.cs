using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions.DiscreteDistributions;
using Perfolizer.Tests.Infra;
using Pragmastat;

namespace Perfolizer.Tests.Mathematics.Distributions.DiscreteDistributions;

public class BinomialDistributionTests
{
    private readonly ITestOutputHelper output;

    public BinomialDistributionTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    private class TestData
    {
        public int N { get; }
        public Probability P { get; }
        public double[] ExpectedPmf { get; }

        public string Key => $"B({N}, {P})";

        public TestData(int n, Probability p, double[] expectedPmf)
        {
            N = n;
            P = p;
            ExpectedPmf = expectedPmf;
        }
    }

    private static readonly List<TestData> TestDataList = new()
    {
        new TestData(1, 0.5, new [] {0.5, 0.5}),
        new TestData(2, 0.5, new [] {0.25, 0.5, 0.25}),
        new TestData(3, 0.5, new [] {0.125, 0.375, 0.375, 0.125}),
        new TestData(4, 0.5, new [] {0.0625, 0.25, 0.375, 0.25, 0.0625}),
        new TestData(5, 0.1, new [] {0.59049, 0.32805, 0.0729, 0.0081, 0.000450000000000001, 1e-05}),
        new TestData(5, 0.2, new [] {0.32768, 0.4096, 0.2048, 0.0512, 0.0064, 0.00032}),
        new TestData(5, 0.3, new [] {0.16807, 0.36015, 0.3087, 0.1323, 0.02835, 0.00243}),
        new TestData(5, 0.4, new [] {0.07776, 0.2592, 0.3456, 0.2304, 0.0768, 0.01024}),
        new TestData(5, 0.5, new [] {0.03125, 0.15625, 0.3125, 0.3125, 0.15625, 0.03125}),
    };

    [UsedImplicitly]
    public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataList.Select(it => it.Key));

    [Theory]
    [MemberData(nameof(TestDataKeys))]
    public void Test(string testDataKey)
    {
        var testData = TestDataList.First(it => it.Key == testDataKey);
        int n = testData.N;
        var p = testData.P;
        var distribution = new BinomialDistribution(n, p);
        var comparer = new AbsoluteEqualityComparer(1e-7);
        for (int k = 0; k < n; k++)
        {
            double actualPmf = distribution.Pmf(k);
            double expectedPmf = testData.ExpectedPmf[k];
            output.WriteLine($"PMF({k}) = {actualPmf} (Expected: {expectedPmf})");

            double actualCdf = distribution.Cdf(k);
            double expectedCdf = testData.ExpectedPmf.Take(k + 1).Sum();
            output.WriteLine($"CDF({k}) = {actualCdf} (Expected: {expectedCdf})");

            int actualQuantile = distribution.Quantile(expectedCdf);
            int expectedQuantile = k;
            output.WriteLine($"Quantile({expectedCdf}) = {actualQuantile} (Expected: {expectedQuantile})");

            Assert.Equal(expectedPmf, actualPmf, comparer);
            Assert.Equal(expectedCdf, actualCdf, comparer);
            Assert.Equal(expectedQuantile, actualQuantile, comparer);
        }
    }
}