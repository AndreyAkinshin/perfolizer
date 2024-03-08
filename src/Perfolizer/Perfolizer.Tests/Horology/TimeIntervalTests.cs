using Perfolizer.Horology;

namespace Perfolizer.Tests.Horology;

public class TimeIntervalTests
{
    [Fact]
    public void TimeIntervalAbsTest() =>
        Assert.True(TimeInterval.FromHours(-1).Abs().Equals(TimeInterval.FromHours(1), 1e-9));

    [Fact]
    public void TimeIntervalToFrequencyTest() =>
        Assert.True(TimeInterval.Millisecond.ToFrequency().Equals(Frequency.KHz, 1e-9));

    [Theory]
    [InlineData(0.00001, "0ns")]
    [InlineData(0.001, "0.001ns")]
    [InlineData(0.9280214, "0.928ns")]
    [InlineData(123.4, "123.4ns")]
    [InlineData(1234, "1.234μs")]
    [InlineData(1234567, "1.235ms")]
    [InlineData(1234567890, "1.235s")]
    [InlineData(60_000_000_000, "1m")]
    [InlineData(5 * 60_000_000_000, "5m")]
    [InlineData(60 * 60_000_000_000, "1h")]
    public void TimeIntervalFormatTest(double ns, string expected)
    {
        var actual = TimeInterval.FromNanoseconds(ns).ToString();
        Assert.Equal(expected, actual);
    }
}