using Perfolizer.Mathematics.Functions;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Mathematics.Functions;

public class BetaFunctionTests
{
    [Fact]
    public void BetaCompleteValue()
    {
        var comparer = new AbsoluteEqualityComparer(0.0000001);
        for (int a = 1; a <= 20; a++)
        for (int b = 1; b <= 20; b++)
        {
            double actual = BetaFunction.CompleteValue(a, b);
            double expected = Factorial(a - 1) * Factorial(b - 1) / Factorial(a + b - 1);
            Assert.Equal(expected, actual, comparer);
        }
    }
        
    [Fact]
    public void BetaCompleteLogValue()
    {
        var comparer = new AbsoluteEqualityComparer(0.0000001);
        for (int a = 1; a <= 20; a++)
        for (int b = 1; b <= 20; b++)
        {
            double actual = BetaFunction.CompleteLogValue(a, b);
            double expected = Math.Log(Factorial(a - 1) * Factorial(b - 1) / Factorial(a + b - 1));
            Assert.Equal(expected, actual, comparer);
        }
    }

    private static double Factorial(int n)
    {
        double result = 1.0;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }
}