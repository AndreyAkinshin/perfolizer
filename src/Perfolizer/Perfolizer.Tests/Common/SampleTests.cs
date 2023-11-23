using Perfolizer.Common;

namespace Perfolizer.Tests.Common;

public class SampleTests
{
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