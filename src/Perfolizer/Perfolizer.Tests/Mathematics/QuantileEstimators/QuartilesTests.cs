using System.Diagnostics.CodeAnalysis;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators;

public class QuartilesTests
{
    private void Check(
        IReadOnlyList<double> values,
        IReadOnlyList<double> expectedQuartiles,
        IQuantileEstimator? quantileEstimator)
    {
        var quartiles = Quartiles.Create(values, quantileEstimator);
        Assert.Equal(expectedQuartiles[0], quartiles.Q0);
        Assert.Equal(expectedQuartiles[1], quartiles.Q1);
        Assert.Equal(expectedQuartiles[2], quartiles.Q2);
        Assert.Equal(expectedQuartiles[3], quartiles.Q3);
        Assert.Equal(expectedQuartiles[4], quartiles.Q4);

        Assert.Equal(expectedQuartiles[0], quartiles.Min);
        Assert.Equal(expectedQuartiles[2], quartiles.Median);
        Assert.Equal(expectedQuartiles[4], quartiles.Max);
        Assert.Equal(expectedQuartiles[3] - expectedQuartiles[1], quartiles.InterquartileRange);
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void QuartileNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => Quartiles.Create((Sample)null!));
    }

    [Fact]
    public void QuartileTest00()
    {
        Check(new double[] {0, 0, 0, 0, 0}, new double[] {0, 0, 0, 0, 0}, SimpleQuantileEstimator.Instance);
    }
        
    [Fact]
    public void QuartileTest01()
    {
        Check(new double[] {0, 1, 2, 3, 4}, new double[] {0, 1, 2, 3, 4}, SimpleQuantileEstimator.Instance);
    }
        
    [Fact]
    public void QuartileTest02()
    {
        Check(new double[] {0, 1, 2, 3, 4}, new double[] {0, 1, 2, 3, 4}, SimpleQuantileEstimator.Instance);
    }
}