using Perfolizer.Metrology;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Metrology;

/// <summary>
/// Threshold strings are round-tripped through configs and command lines, and their presentation
/// reaches end users: BenchmarkDotNet puts Threshold.ToString() into a column name and a column id.
/// </summary>
public class ThresholdTests
{
    [Theory]
    [InlineData("5%")]
    [InlineData("2.5%")]
    [InlineData("10ms")]
    [InlineData("100ns")]
    [InlineData("1s")]
    [InlineData("5%|10ms")]
    public void RoundTripTest(string s)
    {
        Assert.True(Threshold.TryParse(s, out var threshold), $"Failed to parse '{s}'");
        Assert.Equal(s, threshold.ToString());
    }

    [Theory]
    [InlineData("ru-RU")]
    [InlineData("de-DE")]
    [InlineData("en-US")]
    public void ToStringIsCultureIndependentTest(string cultureName) =>
        CultureScope.Run(cultureName, () =>
        {
            Assert.True(Threshold.TryParse("2.5%", out var threshold));
            Assert.Equal("2.5%", threshold.ToString());
        });
}
