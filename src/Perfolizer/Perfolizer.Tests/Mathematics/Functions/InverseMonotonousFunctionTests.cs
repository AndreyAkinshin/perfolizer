using Perfolizer.Mathematics.Functions;
using Perfolizer.Tests.Common;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Mathematics.Functions;

public class InverseMonotonousFunctionTests
{
    private const double Eps = 1e-9;
    private readonly AbsoluteEqualityComparer comparer = new AbsoluteEqualityComparer(Eps);
    private readonly ITestOutputHelper output;

    public InverseMonotonousFunctionTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(-30)]
    public void InverseIdenticalTest(double x) => Check(u => u, -100, 100, x);
        
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(-30)]
    public void InverseNegateTest(double x) => Check(u => -u, -100, 100, x);
        
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(64)]
    public void InverseSquareTest(double x) => Check(u => u * u, 0, 100, x);

    private void Check(Func<double, double> function, double min, double max, double x)
    {
        var inverseFunction = new InverseMonotonousFunction(function, min, max);
        double y = function(x);
        double z = inverseFunction.Value(y);
            
        output.WriteLine($"x = {x}");
        output.WriteLine($"F(x) = {y}");
        output.WriteLine($"F^{{-1}}(F(x)) = {z}");
        Assert.Equal(x, z, comparer);
    }
}