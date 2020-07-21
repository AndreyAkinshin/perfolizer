using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using static System.Math;

namespace Perfolizer.Mathematics.Functions
{
    public static class BetaFunction
    {
        /// <summary>
        /// Complete beta function B(a,b)
        /// </summary>
        public static double CompleteValue(double a, double b)
        {
            return GammaFunction.Value(a) * GammaFunction.Value(b) / GammaFunction.Value(a + b);
        }

        /// <summary>
        /// Natural logarithm of Complete beta function B(a,b)
        /// </summary>
        public static double CompleteLogValue(double a, double b)
        {
            return GammaFunction.LogValue(a) + GammaFunction.LogValue(b) - GammaFunction.LogValue(a + b);
        }

        /// <summary>
        /// Incomplete beta function B(x; a, b)
        /// </summary>
        public static double IncompleteValue(double a, double b, double x)
        {
            return x.Pow(a) / a * HypergeometricFunction.Value(a, 1 - b, a + 1, x, 20);
        }

        /// <summary>
        /// Incomplete beta function B(x; a, b)
        /// </summary>
        public static double IncompleteLogValue(double a, double b, double x)
        {
            return a * Log(x) - Log(a) + Log(HypergeometricFunction.Value(a, 1 - b, a + 1, x, (int) Round(b)));
        }

        /// <summary>
        /// Regularized incomplete beta function Ix(a, b)
        /// </summary>
        public static double RegularizedIncompleteValue(double a, double b, double x)
        {
            // The implementation is inspired by "Incomplete Beta Function in C" (Lewis Van Winkle, 2017)
            // See https://codeplea.com/incomplete-beta-function-c for details
            //
            // We calculate the regularized incomplete beta function using a continued fraction (https://dlmf.nist.gov/8.17#v):
            //   Ix(a, b) = x^a * (1-x)^b / (a*B(a, b)) * 1 / (1 + d[1] / (1 + d[2] / (1 + d[3] / (...))))
            // where
            //   d[2m]   = m(b-m)x / (a+2m-1)(a+2m)
            //   d[2m+1] = -(a+m)(a+b+m)x / (a+2m)(a+2m+1)
            //
            // The approximated value of the continued fraction is calculated using the Lentz's algorithm

            Assertion.NonNegative(nameof(a), a);
            Assertion.NonNegative(nameof(b), b);

            const double eps = 1e-8;
            if (x < eps)
                return 0;
            if (x > 1 - eps)
                return 1;
            if (a < eps && b < eps)
                return 0.5;
            if (a < eps)
                return 1;
            if (b < eps)
                return 0;

            // According to https://dlmf.nist.gov/8.17#v, the continued fraction converges rapidly for x<(a+1)/(a+b+2)
            // If x>=(a+1)/(a+b+2), we use Ix(a, b) = I{1-x}(b, a)
            if (x > (a + 1) / (a + b + 2))
                return 1.0 - RegularizedIncompleteValue(b, a, 1 - x);

            // We use the Lentz's algorithm to calculate the continued fraction
            //   f = 1 + d[1] / (1 + d[2] / (1 + d[3] / (...)))
            // The implementation is based on the following formulas:
            //   u[0] = 1, v[0] = 0, f[0] = 1
            //   u[i] = 1 + d[i] / u[i - 1]
            //   v[i] = 1 / (1 + d[i] * v[i - 1])
            //   f[i] = f[i - 1] * u[i] * v[i]

            const int maxIterationCount = 300;
            static double Normalize(double z) => Abs(z) < 1e-30 ? 1e-30 : z; // Normalization prevents getting zero values

            double u = 1, v = 0, f = 1;
            for (int i = 0; i <= maxIterationCount; i++)
            {
                double d; // d[i]
                int m = i / 2;
                if (i == 0)
                    d = 1.0; // d[0]
                else if (i % 2 == 0)
                    d = m * (b - m) * x / ((a + 2 * m - 1) * (a + 2 * m)); // d[2m]
                else
                    d = -((a + m) * (a + b + m) * x) / ((a + 2.0 * m) * (a + 2.0 * m + 1)); // d[2m+1]

                u = Normalize(1 + d / u);
                v = 1 / Normalize(1 + d * v);
                double uv = u * v;
                f *= uv;

                if (Abs(uv - 1) < eps)
                    break;
            }

            // Ix(a, b) = x^a * (1-x)^b / (a*B(a, b)) * 1 / (1 + d[1] / (1 + d[2] / (1 + d[3] / (...))))
            return Exp(Log(x) * a + Log(1.0 - x) * b - CompleteLogValue(a, b)) / a * (f - 1);
        }
    }
}