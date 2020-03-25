using System;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using static System.Math;

namespace Perfolizer.Mathematics.Distributions
{
    public class BetaDistribution
    {
        public double Alpha { get; }
        public double Beta { get; }

        public BetaDistribution(double alpha, double beta)
        {
            if (alpha < 0)
                throw new ArgumentOutOfRangeException(nameof(alpha), $"{nameof(alpha)} should be non-negative");
            if (beta < 0)
                throw new ArgumentOutOfRangeException(nameof(beta), $"{nameof(beta)} should be non-negative");

            Alpha = alpha;
            Beta = beta;
        }

        /// <summary>
        /// Probability density function 
        /// </summary>
        public double Pdf(double x)
        {
            if (x < 0 || x > 1)
                return 0;

            if (x < 1e-9)
            {
                if (Alpha > 1)
                    return 0;
                if (Abs(Alpha - 1) < 1e-9)
                    return Beta;
                return double.PositiveInfinity;
            }

            if (x > 1 - 1e-9)
            {
                if (Beta > 1)
                    return 0;
                if (Abs(Beta - 1) < 1e-9)
                    return Alpha;
                return double.PositiveInfinity;
            }

            return Exp((Alpha - 1) * Log(x) + (Beta - 1) * Log(1 - x) - BetaFunction.CompleteLogValue(Alpha, Beta));
        }

        /// <summary>
        /// Cumulative distribution function
        /// </summary>
        public double Cdf(double x) => BetaFunction.RegularizedIncompleteValue(Alpha, Beta, x);

        public double Mean => Alpha / (Alpha + Beta);
        public double Variance => Alpha * Beta / (Alpha + Beta).Sqr() / (Alpha + Beta + 1);
        public double StdDev => Variance.Sqrt();
        public double Skewness => 2 * (Beta - Alpha) * Sqrt(Alpha + Beta + 1) / (Alpha + Beta + 2) / Sqrt(Alpha * Beta);
    }
}