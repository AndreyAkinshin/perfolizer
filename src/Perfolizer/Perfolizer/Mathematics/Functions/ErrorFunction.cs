using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using static System.Math;

namespace Perfolizer.Mathematics.Functions
{
    /// <summary>
    /// Gauss error function erf
    ///
    /// <remarks>
    /// erf(z) = 2 / \sqrt{\pi} \int_0^z e^{-t^2} dt
    /// </remarks>
    /// </summary>
    public static class ErrorFunction
    {
        /// <summary>
        /// The value of the error function
        ///
        /// <remarks>
        /// Numerical approximation with maximum error: 1.5 * 10^(-7)
        /// See Abramowitz and Stegun, equation 7.1.26
        /// </remarks>
        /// </summary>
        public static double Value(double x)
        {
            const double p = 0.3275911;
            const double a1 = 0.254829592;
            const double a2 = -0.284496736;
            const double a3 = 1.421413741;
            const double a4 = -1.453152027;
            const double a5 = 1.061405429;

            if (x < 0)
                return -Value(-x);

            double t = 1 / (1 + p * x);
            return 1 - (a1 * t + a2 * t * t + a3 * t * t * t + a4 * t * t * t * t + a5 * t * t * t * t * t) * Exp(-x * x);
        }

        /// <summary>
        /// The value of the inverse error function
        ///
        /// <remarks>
        /// Numerical recipes, 3rd ed., page 265
        /// </remarks>
        /// </summary>
        public static double InverseValue(double p)
        {
            Assertion.InRangeExclusive(nameof(p), p, -1, 1);

            p = 1 - p;
            double pp = p < 1.0 ? p : 2 - p;
            double t = Sqrt(-2 * Log(pp / 2));
            double x = -0.70711 * ((2.30753 + t * 0.27061) / (1 + t * (0.99229 + t * 0.04481)) - t);
            for (int i = 0; i < 2; i++)
            {
                double err = (1 - Value(x)) - pp;
                x += err / (1.12837916709551257 * Exp(-x.Sqr()) - x * err);
            }
            return p < 1.0 ? x : -x;
        }
    }
}