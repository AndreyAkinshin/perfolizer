using Perfolizer.Metrology;

namespace Perfolizer.Tests.Metrology;

public class MetrologyTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("2ns")]
    [InlineData("3.5\u03BCs")]
    [InlineData("4ms")]
    [InlineData("5s")]
    [InlineData("6m")]
    [InlineData("7h")]
    [InlineData("8d")]
    [InlineData("9B")]
    [InlineData("10KB")]
    [InlineData("11MB")]
    [InlineData("12GB")]
    [InlineData("13TB")]
    [InlineData("14%")]
    [InlineData("15ES")]
    [InlineData("16x")]
    [InlineData("17Hz")]
    [InlineData("18KHz")]
    [InlineData("19MHz")]
    [InlineData("20GHz")]
    public void MeasurementUnitToStringParseTest(string s)
    {
        if (!Measurement.TryParse(s, out var value))
            throw new Exception($"Failed to parse '{s}'");
        Assert.Equal(s, value.ToString());
    }
}