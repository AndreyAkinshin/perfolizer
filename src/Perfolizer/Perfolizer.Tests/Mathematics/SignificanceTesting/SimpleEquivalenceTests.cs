using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.SignificanceTesting;
using Perfolizer.Mathematics.SignificanceTesting.MannWhitney;
using Perfolizer.Metrology;
using Pragmastat.Metrology;

namespace Perfolizer.Tests.Mathematics.SignificanceTesting;

public class SimpleEquivalenceTests
{
    [Theory]
    [InlineData("[1,2,3,4,5,6,7,8,9,10]", "[1,2,3,4,5,6,7,8,9,10]", "1", ComparisonResult.Indistinguishable)]
    [InlineData("[195,195,196,196]", "[200.3279,200.3178,200.4046]", "1", ComparisonResult.Indistinguishable)]
    [InlineData(
        "[0.819,0.62,-0.742,0.572,1.43,-0.272,-0.34]",
        "[-0.539,-0.311,-0.174,1.186,-1.455,1.021,0.021]",
        "0.1", ComparisonResult.Indistinguishable)]
    [InlineData(
        "[200.3279, 200.3178, 200.4046, 200.3279, 200.3178, 200.4046, 200.3279, 200.3178, 200.4046, 200.3279, 200.3178, 200.4046]",
        "[195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196, 195, 196]",
        "2%", ComparisonResult.Greater)]
    [InlineData(
        "[10070400,10073300,10073500]",
        "[9.7031,9.2344,10.1719,8.6094,9.5469,8.1406,8.6094,8.4531,7.9844,7.2031,8.1406,8.6094,9.0781,8.9219,9.2344,8.9219]",
        "2%", ComparisonResult.Greater)]
    public void TostTest(string x, string y, string threshold, ComparisonResult expected)
    {
        SimpleEquivalenceTest test = new(MannWhitneyTest.Instance);
        ComparisonResult actual = test.Perform(
            SampleFormatter.Default.Parse(x),
            SampleFormatter.Default.Parse(y),
            Threshold.Parse(threshold),
            SignificanceLevel.P1E5);
        Assert.Equal(expected, actual);
    }
}