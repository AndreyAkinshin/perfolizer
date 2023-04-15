using System.Collections.Generic;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Sequences;
using Perfolizer.Tests.Common;
using Xunit;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators;

public class TrimmedHarrellDavisQuantileEstimatorTests
{
    private static readonly IEqualityComparer<double> Comparer = new AbsoluteEqualityComparer(1e-7);

    [Theory]
    [InlineData(10, 0, 0.3, 0.7, 1)]
    [InlineData(0, 10, 0.3, 0, 0.3)]
    [InlineData(1, 1, 0.5, 0.25, 0.75)]
    [InlineData(3, 3, 0.3, 0.35, 0.65)]
    [InlineData(7, 3, 0.3, 0.5797299, 0.8797299)]
    [InlineData(7, 13, 0.3, 0.1947799, 0.4947799)]
    public void BetaHdiTest(double a, double b, double width, double l, double r)
    {
        var hdi = TrimmedHarrellDavisQuantileEstimator.GetBetaHdi(a, b, width);
        Assert.Equal(l, hdi.L, Comparer);
        Assert.Equal(r, hdi.R, Comparer);
    }

    [Fact]
    public void EstimationTest01()
    {
        var sample = new Sample(new double[] { -3, -2, -1, 0, 1, 2, 3 });
        var probabilities = new Probability[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
        double[] actualQuantiles = TrimmedHarrellDavisQuantileEstimator.SqrtInstance.Quantiles(sample, probabilities);
        double[] expectedQuantiles =
        {
            -3, -2.72276083590394, -2.30045481668633, -1.66479731161074, -0.877210708467137, 2.22044604925031e-16, 0.877210708467138,
            1.66479731161074, 2.30045481668633, 2.72276083590394, 3
        };
        Assert.Equal(expectedQuantiles, actualQuantiles, Comparer);
    }

    [Fact]
    public void EstimationTest02()
    {
        var sample = new Sample(new ArithmeticProgressionSequence(0, 1).GenerateArray(100));
        var probabilities = new Probability[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
        double[] actualQuantiles = TrimmedHarrellDavisQuantileEstimator.SqrtInstance.Quantiles(sample, probabilities);
        double[] expectedQuantiles =
        {
            0, 9.23049888501009, 19.1571295075902, 29.235253395819, 39.3599642036428, 49.5, 59.6400357963572, 69.764746604181,
            79.8428704924098, 89.7695011149899, 99
        };
        Assert.Equal(expectedQuantiles, actualQuantiles, Comparer);
    }

    [Fact]
    public void EstimationTest03()
    {
        var sample = new Sample(new ArithmeticProgressionSequence(0, 1).GenerateArray(1_000_000));
        var probabilities = new Probability[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
        double[] actualQuantiles = TrimmedHarrellDavisQuantileEstimator.SqrtInstance.Quantiles(sample, probabilities);
        double[] expectedQuantiles =
        {
            0, 99999.2066949439, 199999.152626934, 299999.235056257, 399999.360305779, 499999.5, 599999.639694222, 699999.764943743,
            799999.847373066, 899999.793305056, 999999
        };
        Assert.Equal(expectedQuantiles, actualQuantiles, new AbsoluteEqualityComparer(1e-1));
    }
}