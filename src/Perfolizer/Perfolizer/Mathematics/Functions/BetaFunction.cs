using System;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions;

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
            return a * Math.Log(x) - Math.Log(a) + Math.Log(HypergeometricFunction.Value(a, 1 - b, a + 1, x, (int) Math.Round(b)));
        }

        /// <summary>
        /// Regularized incomplete beta function Ix(a, b)
        /// </summary>
        public static double RegularizedIncompleteValue(double a, double b, double x)
        {
            const double eps = 1e-5;

            if (a < -eps)
                throw new ArgumentOutOfRangeException(nameof(a), "a should be positive");
            if (b < -eps)
                throw new ArgumentOutOfRangeException(nameof(b), "b should be positive");

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

            if (a + b > 30) // For huge a+b, we use normal approximation
            {
                // We are trying to make the situation symmetric (|a-b|<1) to improve accuracy of the approximation
                double partialResult = 0;
                while (a < b - 1)
                {
                    partialResult += Math.Exp(GammaFunction.LogValue(a + b)
                                              - GammaFunction.LogValue(a + 1) - GammaFunction.LogValue(b)
                                              + a * Math.Log(x) + (b - 1) * Math.Log(1 - x));
                    a++;
                    b--;
                }
                while (b < a - 1)
                {
                    partialResult -= Math.Exp(GammaFunction.LogValue(a + b)
                                              - GammaFunction.LogValue(b + 1) - GammaFunction.LogValue(a)
                                              + (a - 1) * Math.Log(x) + b * Math.Log(1 - x));
                    a--;
                    b++;
                }

                double mean = a / (a + b);
                double stdDev = Math.Sqrt(a * b / (a + b).Sqr() / (a + b + 1));
                double normalApproximation = new NormalDistribution(mean, stdDev).Cdf(x);
                return partialResult + normalApproximation;
            }

            if (x > 0.5) // Accuracy is better for small x
                return 1 - RegularizedIncompleteValue(b, a, 1 - x);

            double result = Math.Exp(IncompleteLogValue(a, b, x) - CompleteLogValue(a, b));
            return double.IsNaN(result) ? 0 : result.Clamp(0, 1);
        }
    }
}