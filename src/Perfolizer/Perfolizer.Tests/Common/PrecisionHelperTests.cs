using Perfolizer.Mathematics.Common;

namespace Perfolizer.Tests.Common;

public class PrecisionHelperTests
{
    [Theory]
    [InlineData(2, 1.0)]
    [InlineData(1, 10.0)]
    [InlineData(1, 100.0)]
    [InlineData(3, 0.1)]
    [InlineData(4, 0.01)]
    [InlineData(5, 0.001)]
    [InlineData(5, 0.001, 10.0)]
    public void PrecisionHelperTest(int expected, params double[] values)
    {
        int actual = PrecisionHelper.GetOptimalPrecision(values);
        Assert.Equal(expected, actual);
    }
}