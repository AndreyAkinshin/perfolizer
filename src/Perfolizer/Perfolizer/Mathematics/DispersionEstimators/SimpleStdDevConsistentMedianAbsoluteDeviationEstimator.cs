using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.DispersionEstimators
{
    public class SimpleStdDevConsistentMedianAbsoluteDeviationEstimator : MedianAbsoluteDeviationEstimatorBase
    {
        public static IMedianAbsoluteDeviationEstimator Instance = new SimpleStdDevConsistentMedianAbsoluteDeviationEstimator();

        public override IQuantileEstimator QuantileEstimator => SimpleQuantileEstimator.Instance;

        /// <summary>
        /// Factors are taken from the following paper:
        /// Park, Chanseok, Haewon Kim, and Min Wang.
        /// “Investigation of finite-sample properties of robust location and scale estimators.”
        /// Communications in Statistics-Simulation and Computation (2020): 1-27.
        /// https://doi.org/10.1080/03610918.2019.1699114
        /// </summary>
        private static readonly double[] ParkBias =
        {
            double.NaN, double.NaN, -0.163388, -0.3275897, -0.2648275, -0.178125, -0.1594213,
            -0.1210631, -0.1131928, -0.0920658, -0.0874503, -0.0741303, -0.0711412,
            -0.0620918, -0.060021, -0.0534603, -0.0519047, -0.0467319, -0.0455579,
            -0.0417554, -0.0408248, -0.0376967, -0.036835, -0.0342394, -0.033539,
            -0.0313065, -0.0309765, -0.029022, -0.0287074, -0.0269133, -0.0265451,
            -0.0250734, -0.0248177, -0.023646, -0.0232808, -0.0222099, -0.0220756,
            -0.0210129, -0.0207309, -0.0199272, -0.019714, -0.0188446, -0.0188203,
            -0.0180521, -0.0178185, -0.0171866, -0.0170796, -0.0165391, -0.0163509,
            -0.0157862, -0.0157372, -0.015282, -0.0149951, -0.0146042, -0.0145007,
            -0.0140391, -0.0139674, -0.0136336, -0.0134819, -0.0130812, -0.0129708,
            -0.0126589, -0.0125598, -0.0122696, -0.0121523, -0.0118163, -0.0118244,
            -0.0115177, -0.0114479, -0.0111309, -0.0110816, -0.0108875, -0.0108319,
            -0.0106032, -0.0105424, -0.0102237, -0.0102132, -0.0099408, -0.0099776,
            -0.0097815, -0.0097399, -0.0094837, -0.0094713, -0.009239, -0.0092875,
            -0.0091508, -0.0090145, -0.0088191, -0.0088205, -0.0086622, -0.0085714,
            -0.0084718, -0.0083861, -0.0082559, -0.008265, -0.0080977, -0.0080708,
            -0.007881, -0.0078492, -0.0077043, -0.0077614
        };

        public static double GetScaleFactor(int n)
        {
            double bias = n < ParkBias.Length - 1 ? ParkBias[n] : -0.76213 / n - 0.86413 / n / n;
            return 1 / (0.674489750196082 * (1 + bias));
        }

        protected override double GetScaleFactor(Sample sample)
        {
            return GetScaleFactor(sample.Count);
        }
    }
}