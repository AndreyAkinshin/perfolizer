using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.SignificanceTesting;
using Perfolizer.Mathematics.SignificanceTesting.Base;
using Perfolizer.Mathematics.Thresholds;
using Perfolizer.Tests.Common;

namespace Perfolizer.Tests.Mathematics.SignificanceTesting;

public class BrunnerMunzelTests
{
    private readonly ITestOutputHelper output;

    public BrunnerMunzelTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void BrunnerMunzelTest01()
    {
        double[] x =
        {
            -0.82148991539243, -0.374792175133262, -1.03342868703527, 0.925795747147231,
            0.63590395565913, 1.52159965673654, -1.18934655139401, -0.854560137900411,
            0.535023079812285, 2.35417241954913
        };
        double[] y =
        {
            -0.901325307195209, 0.425266231692299, -0.360893100160856,
            0.162706180166306, 0.39610552209069, 0.475742128720794, 1.28370747104077,
            0.64604784451729, 1.3787545289053, 0.5382592425226
        };
        CheckGreater(x, y, -0.416470332044473, 12.126760041271, 0.657828745852485);
    }

    [Fact]
    public void BrunnerMunzelTest02()
    {
        double[] x =
        {
            -1.1301745064274, 0.245290511406796, 0.050549376587986, -1.2622003420208,
            0.310929350954766, 0.450629844803094, 0.289116269077706, -1.88168159346884,
            1.05104251203298, -1.26283215137108
        };
        double[] y =
        {
            -0.823392258951573, 2.23621559548957, 0.393710501684194, 1.27771213304125,
            -0.137278818255678, 0.314813651354042, -0.299275281838485, -2.12995868447167,
            1.36621623679248, -1.24332266661598
        };
        CheckGreater(x, y, -0.810441509480792, 15.9157043954192, 0.78518059445665);
    }

    [Fact]
    public void BrunnerMunzelTest03()
    {
        double[] x = { 1.0, 2.0, 3.0 };
        double[] y = { 4.0, 5.0, 6.0 };
        CheckGreater(x, y, double.NegativeInfinity, double.NaN, 1);
    }

    [Fact]
    public void BrunnerMunzelTest04()
    {
        double[] x = { 4.0, 5.0, 6.0 };
        double[] y = { 1.0, 2.0, 3.0 };
        CheckGreater(x, y, double.PositiveInfinity, double.NaN, 0);
    }

    [Fact]
    public void BrunnerMunzelTest05()
    {
        double[] x =
        {
            1.11889910876363, -0.209134066431649, -0.757517622272587, -0.613534373567174,
            -0.355363651628834
        };
        double[] y =
        {
            0.235124757189047, 0.306014243849478, 0.00910452289150105,
            -1.13668728998333, -2.10451608404323, 0.961584942150633, -0.388761918200748
        };
        CheckGreater(x, y, -0.0730621536215222, 9.85667474591963, 0.528390995436648);
    }

    [AssertionMethod]
    private void CheckGreater(double[] x, double[] y, double w, double df, double pValue)
    {
        var threshold = AbsoluteThreshold.Zero;
        var result = BrunnerMunzelTest.Instance.Perform(x.ToSample(), y.ToSample(), AlternativeHypothesis.Greater, threshold);
        if (result == null)
            throw new NullReferenceException($"{nameof(BrunnerMunzelTest)} returned null");
        output.WriteLine("W       = " + result.W + " (Expected: " + w + ")");
        output.WriteLine("df      = " + result.Df + " (Expected: " + df + ")");
        output.WriteLine("p-value = " + result.PValue + " (Expected: " + pValue + ")");

        Assert.Equal(w, result.W, AbsoluteEqualityComparer.E9);
        Assert.Equal(df, result.Df, AbsoluteEqualityComparer.E9);
        Assert.Equal(pValue, result.PValue, AbsoluteEqualityComparer.E9);
    }
}