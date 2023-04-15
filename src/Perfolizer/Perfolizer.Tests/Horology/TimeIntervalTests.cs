using Perfolizer.Horology;
using Xunit;

namespace Perfolizer.Tests.Horology;

public class TimeIntervalTests
{
    [Fact]
    public void TimeIntervalAbsTest() => Assert.True(TimeInterval.FromHours(-1).Abs().Equals(TimeInterval.FromHours(1), 1e-9));

    [Fact]
    public void TimeIntervalToFrequencyTest() => Assert.True(TimeInterval.Millisecond.ToFrequency().Equals(Frequency.KHz, 1e-9));
}