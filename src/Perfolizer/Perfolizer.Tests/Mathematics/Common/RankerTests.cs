using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Tests.Common;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Mathematics.Common;

public class RankerTests(ITestOutputHelper output)
{
    [AssertionMethod]
    private void Check(double[] x, double[] expectedRanks)
    {
        double[] actualRanks = Ranker.Instance.GetRanks(x);

        string Present(double[] values) => new Sample(values).ToString();

        output.WriteLine("Input         : " + Present(x));
        output.WriteLine("ExpectedRanks : " + Present(expectedRanks));
        output.WriteLine("ActualRanks   : " + Present(actualRanks));

        Assert.Equal(expectedRanks, actualRanks, AbsoluteEqualityComparer.E9);
    }

    private void DoSimpleTest(int n, Func<double, double> transform)
    {
        double[] x = Enumerable.Range(1, n).Select(value => (double)value).ToArray();
        int permutationCount = 0;

        do
        {
            permutationCount++;
            Check(x.Select(transform).ToArray(), x);
            output.WriteLine();
        } while (Permutator.NextPermutation(x));

        Assert.Equal(FactorialFunction.Value(n), permutationCount);
    }

    [Fact]
    public void RankIdentityTest01() => DoSimpleTest(5, x => x);

    [Fact]
    public void RankIdentityTest02() => DoSimpleTest(6, x => x * x);

    [Fact]
    public void RankTestTies01() => Check(new double[] { 1, 1, 1, 2, 2, 3 }, new[] { 2, 2, 2, 4.5, 4.5, 6 });

    [Fact]
    public void RankTestTies02() => Check(new double[] { 1, 1, 2, 2, 3, 1 }, new[] { 2, 2, 4.5, 4.5, 6, 2 });

    [Fact]
    public void RankTestTies03() =>
        Check(new double[] { 1, 1, 2, 2, 2, 2, 2, 3, 1 }, new double[] { 2, 2, 6, 6, 6, 6, 6, 9, 2 });
}