using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;
using static System.Math;

namespace Perfolizer.Mathematics.Distributions
{
    public class NormalDistribution
    {
        public static readonly NormalDistribution Standard = new NormalDistribution(0, 1);

        private readonly double mean, sd;

        public NormalDistribution(double mean, double sd)
        {
            this.mean = mean;
            this.sd = sd;
        }

        /// <summary>
        /// Probability density function 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Pdf(double x) => Exp(-((x - mean) / sd).Sqr() / 2) / (sd * Sqrt(2 * PI));

        /// <summary>
        /// Cumulative distribution function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Cdf(double x) => mean + Gauss(x) * sd;

        /// <summary>
        /// ACM Algorithm 209: Gauss
        ///
        /// Calculates $(1/\sqrt{2\pi}) \int_{-\infty}^x e^{-u^2 / 2} du$
        /// by means of polynomial approximations due to A. M. Murray of Aberdeen University;
        ///
        /// See: http://dl.acm.org/citation.cfm?id=367664
        /// </summary>
        /// <param name="x">-infinity..+infinity</param>
        /// <returns>Area under the Standard Normal Curve from -infinity to x</returns>
        [PublicAPI]
        public static double Gauss(double x)
        {
            double z;
            if (Abs(x) < 1e-9)
                z = 0.0;
            else
            {
                double y = Abs(x) / 2;
                if (y >= 3.0)
                    z = 1.0;
                else if (y < 1.0)
                {
                    double w = y * y;
                    z = ((((((((0.000124818987 * w - 0.001075204047) * w + 0.005198775019) * w - 0.019198292004) * w +
                             0.059054035642) * w - 0.151968751364) * w + 0.319152932694) * w - 0.531923007300) * w +
                         0.797884560593) * y * 2.0;
                }
                else
                {
                    y = y - 2.0;
                    z = (((((((((((((-0.000045255659 * y + 0.000152529290) * y - 0.000019538132) * y - 0.000676904986) *
                                   y + 0.001390604284) * y - 0.000794620820) * y - 0.002034254874) * y +
                               0.006549791214) *
                              y - 0.010557625006) * y + 0.011630447319) * y - 0.009279453341) * y + 0.005353579108) *
                          y -
                          0.002141268741) * y + 0.000535310849) * y + 0.999936657524;
                }
            }

            return x > 0.0 ? (z + 1.0) / 2 : (1.0 - z) / 2;
        }
    }
}