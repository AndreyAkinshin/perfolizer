using Perfolizer.Mathematics.Functions;
using Perfolizer.Tests.Common;
using Xunit;

namespace Perfolizer.Tests.Mathematics.Functions;

public class ErrorFunctionTests
{
    [Theory]
    [InlineData(-3, -0.9999779)]
    [InlineData(-2, -0.9953223)]
    [InlineData(-1, -0.8427008)]
    [InlineData(0, 0)]
    [InlineData(1, 0.8427008)]
    [InlineData(2, 0.9953223)]
    [InlineData(3, 0.9999779)]
    public void ErrorFunctionValueTest(double x, double expectedValue)
    {
        double actualValue = ErrorFunction.Value(x);
        Assert.Equal(expectedValue, actualValue, new AbsoluteEqualityComparer(1e-6));
    }

    [Theory]
    [InlineData(-0.9, -1.16308715)]
    [InlineData(-0.5, -0.47693628)]
    [InlineData(-0.1, -0.08885599)]
    [InlineData(0, 0)]
    [InlineData(0.1, 0.08885599)]
    [InlineData(0.5, 0.47693628)]
    [InlineData(0.9, 1.16308715)]
    public void ErrorFunctionInverseValueTest(double x, double expectedValue)
    {
        double actualValue = ErrorFunction.InverseValue(x);
        Assert.Equal(expectedValue, actualValue, new AbsoluteEqualityComparer(1e-6));
    } 
}