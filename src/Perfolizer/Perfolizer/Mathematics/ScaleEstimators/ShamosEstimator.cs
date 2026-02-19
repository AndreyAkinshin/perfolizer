using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Pragmastat;

namespace Perfolizer.Mathematics.ScaleEstimators;

/// <summary>
/// Original work:
/// * Shamos, Michael Ian. “Geometry and Statistics: Problems at the Interface.” In Algorithms and Complexity. 1977.
///
/// Comparison of the Shamos estimator to the Median Absolute Deviation and the Rousseeuw-Croux Qn scale estimators:
/// * https://aakinshin.net/posts/mad-vs-shamos/
/// * https://aakinshin.net/posts/shamos-vs-qn/
/// </summary>
public class ShamosEstimator : IScaleEstimator
{
    public static readonly ShamosEstimator Instance = new();

    private const double AsymptoticBias = 0.9538726; // Φ(0.75) * sqrt(2)

    /**
     * The bias factor values were taken from Table A2 (Page 17) of the following paper:
     * * Park, Chanseok, Haewon Kim, and Min Wang.
     *   “Investigation of finite-sample properties of robust location and scale estimators.”
     *   Communications in Statistics-Simulation and Computation (2020): 1-27.
         https://doi.org/10.1080/03610918.2019.1699114
     */
    private readonly double[] biasFactors =
    [
        double.NaN, double.NaN, 0.1831500, 0.2989400, 0.1582782, 0.1011748, 0.1005038, 0.0676993, 0.0609574, 0.0543760
        , 0.0476839, 0.0426722, 0.0385003, 0.0353028, 0.0323526, 0.0299677, 0.0280421, 0.0262195, 0.0247674, 0.0232297
        , 0.0220155, 0.0208687, 0.0199446, 0.0189794, 0.0182343, 0.0174421, 0.0166364, 0.0160158, 0.0153715, 0.0148940
        , 0.0144027, 0.0138855, 0.0134510, 0.0130228, 0.0127183, 0.0122444, 0.0118214, 0.0115469, 0.0113206, 0.0109636
        , 0.0106308, 0.0104384, 0.0100693, 0.0098523, 0.0096735, 0.0094973, 0.0092210, 0.0089781, 0.0088083, 0.0086574
        , 0.0084772, 0.0082120, 0.0081874, 0.0079775, 0.0078126, 0.0076743, 0.0075212, 0.0074051, 0.0072528, 0.0071807
        , 0.0070617, 0.0069123, 0.0067833, 0.0066439, 0.0065821, 0.0064889, 0.0063844, 0.0062930, 0.0061910, 0.0061255
        , 0.0060681, 0.0058994, 0.0058235, 0.0057172, 0.0056805, 0.0056343, 0.0055605, 0.0055011, 0.0053872, 0.0053062
        , 0.0052348, 0.0052075, 0.0051173, 0.0050697, 0.0049805, 0.0048705, 0.0048695, 0.0048287, 0.0047315, 0.0046961
        , 0.0046698, 0.0046010, 0.0045544, 0.0045191, 0.0044245, 0.0044074, 0.0043579, 0.0043536, 0.0042874, 0.0042520
        , 0.0041864
    ];

    /// <summary>
    /// Returns the scale factor to make the Shamos estimator consistent with the standard deviation under normality.
    /// </summary>
    /// <param name="n">The sample size</param>
    /// <returns>The scale factor</returns>
    private double Factor(int n) => n switch
    {
        <= 1 => double.NaN,
        <= 100 => 1 / (AsymptoticBias * (1 + biasFactors[n]))
        ,
        _ => 1 / (AsymptoticBias * (1 + 0.414253297 / n - 0.442396799 / n / n))
    };

    public double Scale(Sample x)
    {
        double raw = PairwiseEstimatorHelper
            .Estimate(x, (xi, xj) => Abs(xi - xj), SimpleQuantileEstimator.Instance, Probability.Half, false);
        return raw * Factor(x.Size);
    }
}