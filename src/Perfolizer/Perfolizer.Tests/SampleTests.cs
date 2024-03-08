namespace Perfolizer.Tests;

public class SampleTests
{
    [Theory]
    [InlineData("[1,2,3]ms", "[4]s", "[1,2,3,4000]ms")]
    [InlineData("[2000]KB", "[4]B", "[2048000,4]B")]
    public void SampleConcatTest(string a, string b, string c)
    {
        string actual = Sample.Parse(a).Concat(Sample.Parse(b)).ToString();
        string expected = Sample.Parse(c).ToString();
        Assert.Equal(expected, actual);
    }
}