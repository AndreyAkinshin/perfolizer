using Perfolizer.Horology;

namespace Perfolizer.Tests.Metrology;

public class TimeUnitTests(ITestOutputHelper output)
{
    [Fact]
    public void ConvertTest()
    {
        CheckConvertTwoWay(1000, TimeUnit.Nanosecond, 1, TimeUnit.Microsecond);
        CheckConvertTwoWay(1000, TimeUnit.Microsecond, 1, TimeUnit.Millisecond);
        CheckConvertTwoWay(1000, TimeUnit.Millisecond, 1, TimeUnit.Second);
        CheckConvertTwoWay(60, TimeUnit.Second, 1, TimeUnit.Minute);
        CheckConvertTwoWay(60, TimeUnit.Minute, 1, TimeUnit.Hour);
        CheckConvertTwoWay(24, TimeUnit.Hour, 1, TimeUnit.Day);
    }

    [Theory]
    // ns
    [InlineData("ns", 001.0)]
    [InlineData("ns", 100.0)]
    [InlineData("ns", 999.0)]
    // us
    [InlineData("us", 001_000.0)]
    [InlineData("us", 100_000.0)]
    [InlineData("us", 999_000.0)]
    // ms
    [InlineData("ms", 001_000_000.0)]
    [InlineData("ms", 100_000_000.0)]
    [InlineData("ms", 999_000_000.0)]
    // s
    [InlineData("s", 01_000_000_000.0)]
    [InlineData("s", 59_000_000_000.0)]
    // m
    [InlineData("m", 0060_000_000_000.0)]
    [InlineData("m", 3599_000_000_000.0)]
    // h
    [InlineData("h", 3600_000_000_000.0)]
    [InlineData("h", 23 * 3600_000_000_000.0)]
    // d
    [InlineData("d", 24 * 3600_000_000_000.0)]
    public void GetBestTimeUnitTest(string expectedUnit, double value) =>
        CheckGetBestTimeUnit(TimeUnit.Parse(expectedUnit), value);

    private void CheckGetBestTimeUnit(TimeUnit timeUnit, params double[] values)
    {
        output.WriteLine($"Best TimeUnit for ({string.Join(";", values)})ns is {timeUnit.FullName}");
        Assert.Equal(timeUnit.FullName, TimeUnit.GetBestTimeUnit(values).FullName);
    }

    private void CheckConvertTwoWay(double value1, TimeUnit unit1, double value2, TimeUnit unit2)
    {
        CheckConvertOneWay(value1, unit1, value2, unit2);
        CheckConvertOneWay(value2, unit2, value1, unit1);
    }

    private void CheckConvertOneWay(double value1, TimeUnit unit1, double value2, TimeUnit unit2)
    {
        double convertedValue2 = TimeUnit.Convert(value1, unit1, unit2);
        output.WriteLine($"Expected: {value1} {unit1.Abbreviation} = {value2} {unit2.Abbreviation}");
        output.WriteLine($"Actual: {value1} {unit1.Abbreviation} = {convertedValue2} {unit2.Abbreviation}");
        output.WriteLine("");
        Assert.Equal(value2, convertedValue2, 4);
    }
}