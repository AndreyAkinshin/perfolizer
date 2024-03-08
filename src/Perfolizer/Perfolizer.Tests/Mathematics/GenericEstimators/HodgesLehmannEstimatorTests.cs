using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Mathematics.GenericEstimators;

public class HodgesLehmannEstimatorTests
{
    private static readonly IEqualityComparer<double> EqualityComparer = AbsoluteEqualityComparer.E9;

    [Fact]
    public void HodgesLehmannLocationShiftTest01()
    {
        CheckShift(
            Array(0.298366502872861, 2.30272056972301, -1.07018041144338, 0.967248885283515, -0.849008187096325),
            Array(9.98634587887872, 8.78621971483415, 9.35864761227285, 9.80372149505987, 10.2586337161638),
            -9.50535499218701
        );
    }

    [Fact]
    public void HodgesLehmannWeightedLocationShiftTest01()
    {
        CheckShift(
            Array(0.298366502872861, 2.30272056972301, -1.07018041144338, 0.967248885283515, -0.849008187096325),
            Array(9.98634587887872, 8.78621971483415, 9.35864761227285, 9.80372149505987, 10.2586337161638),
            Array(1, 1, 1, 1, 1),
            Array(1, 1, 1, 1, 1),
            -9.50535499218701
        );
    }

    [Fact]
    public void HodgesLehmannMedianTest01()
    {
        CheckMedian(Array(1, 2, 3, 4, 5), 3);
    }

    [Fact]
    public void HodgesLehmannMedianTest02()
    {
        CheckMedian(Array(-0.616152614118394, -1.16812505107469, 0.328640358591086, 1.46651062744016, -0.356009545088913),
            -0.143756127763654);
    }

    [Fact]
    public void HodgesLehmannWeightedMedianTest01()
    {
        CheckMedian(Array(1, 2, 3, 4, 5), Array(1, 1, 1, 1, 1), 3);
    }

    [Fact]
    public void HodgesLehmannWeightedMedianTest02()
    {
        CheckMedian(Array(1, 2, 3), Array(0, 0, 1), 3);
    }

    [Fact]
    public void HodgesLehmannWeightedMedianTest03()
    {
        CheckMedian(Array(1, 2, 3, 4, 5), Array(1, 1, 1, 0, 0), 2);
    }

    [Fact]
    public void HodgesLehmannWeightedMedianTest04()
    {
        CheckMedian(Array(1, 2, 3, 4, 5), Array(1, 1, 1, 0, 0.1), 2);
    }

    [Fact]
    public void HodgesLehmannNonSupportedWeightedCase()
    {
        var estimator = new HodgesLehmannEstimator(HyndmanFanQuantileEstimator.Type2);
        var x = new Sample(Array(1, 2), Array(0.5, 0.6));
        Assert.Throws<WeightedSampleNotSupportedException>(() => estimator.Median(x));
    }

    [Fact]
    public void HodgesLehmannRatioTest01()
    {
        CheckRatio(Array(6, 6, 6, 6), Array(2, 2, 2, 2), 3.0);
    }

    [Fact]
    public void HodgesLehmannRatioTest02()
    {
        CheckRatio(Array(2, 4, 6, 8), Array(1, 2, 3, 4), 2.0);
    }

    [Fact]
    public void HodgesLehmannRatioTest03()
    {
        CheckRatio(Array(1, 5, 10), Array(3, 5, 7), 1.0);
    }

    private static void CheckShift(double[] x, double[] y, double expected)
    {
        double actual = HodgesLehmannEstimator.Instance.Shift(x.ToSample(), y.ToSample());
        Assert.Equal(expected, actual, EqualityComparer);
    }

    private static void CheckShift(double[] x, double[] y, double[] xw, double[] yw, double expected)
    {
        double actual = HodgesLehmannEstimator.Instance.Shift(new Sample(x, xw), new Sample(y, yw));
        Assert.Equal(expected, actual, EqualityComparer);
    }

    private static void CheckMedian(double[] x, double expected)
    {
        double actual = HodgesLehmannEstimator.Instance.Median(x.ToSample());
        Assert.Equal(expected, actual, EqualityComparer);
    }

    private static void CheckMedian(double[] x, double[] w, double expected)
    {
        double actual = HodgesLehmannEstimator.Instance.Median(new Sample(x, w));
        Assert.Equal(expected, actual, EqualityComparer);
    }

    private static void CheckRatio(double[] x, double[] y, double expected)
    {
        double actual = HodgesLehmannEstimator.Instance.Ratio(x.ToSample(), y.ToSample());
        Assert.Equal(expected, actual, EqualityComparer);
    }

    private static double[] Array(params double[] values) => values;

    private static double[] Array(params int[] values)
    {
        double[] doubleValues = new double[values.Length];
        for (int i = 0; i < values.Length; i++)
            doubleValues[i] = values[i];
        return doubleValues;
    }
}