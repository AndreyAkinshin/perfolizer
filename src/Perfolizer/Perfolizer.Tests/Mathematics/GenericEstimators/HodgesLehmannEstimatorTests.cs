using System.Collections.Generic;
using Perfolizer.Collections;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Tests.Common;
using Xunit;

namespace Perfolizer.Tests.Mathematics.GenericEstimators;

public class HodgesLehmannEstimatorTests
{
    private static readonly IEqualityComparer<double> EqualityComparer = AbsoluteEqualityComparer.E9;

    [Fact]
    public void HodgesLehmannLocationShiftTest01()
    {
        CheckShift(
            new[] { 0.298366502872861, 2.30272056972301, -1.07018041144338, 0.967248885283515, -0.849008187096325 },
            new[] { 9.98634587887872, 8.78621971483415, 9.35864761227285, 9.80372149505987, 10.2586337161638 },
            -9.50535499218701
        );
    }

    [Fact]
    public void HodgesLehmannMedianTest01()
    {
        CheckMedian(new double[] { 1, 2, 3, 4, 5 }, 3);
    }

    [Fact]
    public void HodgesLehmannMedianTest02()
    {
        CheckMedian(new[]
        {
            -0.616152614118394, -1.16812505107469, 0.328640358591086, 1.46651062744016, -0.356009545088913
        }, -0.143756127763654);
    }

    private static void CheckShift(double[] a, double[] b, double expected)
    {
        double actual = HodgesLehmannEstimator.Instance.LocationShift(a.ToSample(), b.ToSample());
        Assert.Equal(expected, actual, EqualityComparer);
    }

    private static void CheckMedian(double[] a, double expected)
    {
        double actual = HodgesLehmannEstimator.Instance.Median(a.ToSample());
        Assert.Equal(expected, actual, EqualityComparer);
    }
}