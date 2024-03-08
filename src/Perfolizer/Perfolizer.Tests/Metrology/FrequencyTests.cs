using Perfolizer.Horology;

namespace Perfolizer.Tests.Metrology;

public class FrequencyTests
{
    [Theory]
    [InlineData(1, "1Hz")]
    [InlineData(2531248, "2.531248MHz")]
    public void FrequencyToStringTest(int hertz, string expected)
    {
        string actual = new Frequency(hertz).ToString();
        Assert.Equal(expected, actual);
    }
}