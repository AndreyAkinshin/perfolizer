using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.SignificanceTesting;
using Perfolizer.Mathematics.SignificanceTesting.MannWhitney;
using Perfolizer.Metrology;

namespace Perfolizer.Tests.Mathematics.SignificanceTesting;

public class TostTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData("[1,2,3,4,5,6,7,8,9,10]", "[1,2,3,4,5,6,7,8,9,10]", "1", 1e-5, ComparisonResult.Equivalent)]
    [InlineData("[195,195,196,196]", "[200.3279,200.3178,200.4046]", "1", 1e-5, ComparisonResult.Equivalent)]
    [InlineData(
        "[200.3279, 200.3178, 200.4046, 200.3279, 200.3178, 200.4046, 200.3279, 200.3178, 200.4046, 200.3279, 200.3178, 200.4046]",
        "[195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196]",
        "2%", 1e-5, ComparisonResult.Greater)]
    public void TostTest(string x, string y, string threshold, double alpha, ComparisonResult expected)
    {
        var tost = new TostEquivalenceTest<MannWhitneyResult>(MannWhitneyTest.Instance);
        var result = tost.Perform(Sample.Parse(x), Sample.Parse(y), Threshold.Parse(threshold), alpha);
        Assert.Equal(expected, result.ComparisonResult);
    }
}