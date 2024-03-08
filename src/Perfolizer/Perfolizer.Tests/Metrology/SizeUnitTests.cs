using Perfolizer.Metrology;
using Perfolizer.Tests.Common;

namespace Perfolizer.Tests.Metrology;

public class SizeUnitTests(ITestOutputHelper output)
{
    [Fact]
    public void ConvertTest()
    {
        CheckConvertTwoWay(1024, SizeUnit.B, 1, SizeUnit.KB);
        CheckConvertTwoWay(1024, SizeUnit.KB, 1, SizeUnit.MB);
        CheckConvertTwoWay(1024, SizeUnit.MB, 1, SizeUnit.GB);
        CheckConvertTwoWay(1024, SizeUnit.GB, 1, SizeUnit.TB);
        CheckConvertTwoWay(1024L * 1024 * 1024 * 1024, SizeUnit.B, 1, SizeUnit.TB);
    }

    [Theory]
    [InlineData("0B", 0)]
    [InlineData("1B", 1)]
    [InlineData("10B", 10)]
    [InlineData("100B", 100)]
    [InlineData("1000B", 1000)]
    [InlineData("1023B", 1023)]
    [InlineData("1KB", 1024)]
    [InlineData("1KB", 1025)]
    [InlineData("1.07KB", 1100)]
    [InlineData("1.5KB", 1024 + 512)]
    [InlineData("10KB", 10 * 1024)]
    [InlineData("1023KB", 1023 * 1024)]
    [InlineData("1MB", 1024 * 1024)]
    [InlineData("1GB", 1024 * 1024 * 1024)]
    [InlineData("1TB", 1024L * 1024 * 1024 * 1024)]
    public void SizeUnitFormattingTest(string expected, long bytes)
    {
        Assert.Equal(expected, SizeValue.FromBytes(bytes).ToString());
    }

    private void CheckConvertTwoWay(long value1, SizeUnit unit1, long value2, SizeUnit unit2)
    {
        CheckConvertOneWay(value1, unit1, value2, unit2);
        CheckConvertOneWay(value2, unit2, value1, unit1);
    }

    private void CheckConvertOneWay(long value1, SizeUnit unit1, long value2, SizeUnit unit2)
    {
        double convertedValue2 = SizeUnit.Convert(value1, unit1, unit2);
        output.WriteLine($"Expected: {value1} {unit1.Abbreviation} = {value2} {unit2.Abbreviation}");
        output.WriteLine($"Actual: {value1} {unit1.Abbreviation} = {convertedValue2} {unit2.Abbreviation}");
        output.WriteLine("");
        Assert.Equal(value2, convertedValue2, 4);
    }
}