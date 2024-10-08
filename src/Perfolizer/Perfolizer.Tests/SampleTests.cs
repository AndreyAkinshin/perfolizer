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

    [Fact]
    public void SampleCtorTest()
    {
        string expected = "1;2;3";
        string Format(Sample s) => string.Join(";", s.Values);

        Assert.Equal(expected, Format(new Sample(new[] { 1.0, 2.0, 3.0 })));
        Assert.Equal(expected, Format(new Sample(new[] { 1, 2, 3 })));
        Assert.Equal(expected, Format(new Sample(new[] { 1L, 2L, 3L })));
    }
}