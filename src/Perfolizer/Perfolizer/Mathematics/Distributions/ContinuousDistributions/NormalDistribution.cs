using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class NormalDistribution : IContinuousDistribution
{
    public static readonly NormalDistribution Standard = new();

    public double Mean { get; }
    public double StandardDeviation { get; }

    public NormalDistribution(double mean = 0, double stdDev = 1)
    {
        Assertion.Positive(nameof(stdDev), stdDev);

        Mean = mean;
        StandardDeviation = stdDev;
    }

    public double Pdf(double x) => Exp(-((x - Mean) / StandardDeviation).Sqr() / 2) / (StandardDeviation * Sqrt(2 * PI));

    public double Cdf(double x) => Gauss((x - Mean) / StandardDeviation);

    public double Quantile(Probability p)
    {
        return p.Value switch
        {
            0 => double.NegativeInfinity,
            1 => double.PositiveInfinity,
            _ => Mean + StandardDeviation * Constants.Sqrt2 * ErrorFunction.InverseValue(2 * p - 1)
        };
    }

    internal class NormalRandomGenerator : RandomGenerator
    {
        private readonly NormalDistribution distribution;

        public NormalRandomGenerator(NormalDistribution distribution)
        {
            this.distribution = distribution;
        }

        public NormalRandomGenerator(int seed, NormalDistribution distribution) : base(seed)
        {
            this.distribution = distribution;
        }

        public NormalRandomGenerator(Random? random, NormalDistribution distribution) : base(random)
        {
            this.distribution = distribution;
        }

        /// <summary>
        /// Generate next random number from the normal distribution
        /// </summary>
        /// <remarks>
        /// The method uses the Box–Muller transform.
        /// See: Box, George EP. "A note on the generation of random normal deviates." Ann. Math. Stat. 29 (1958): 610-611.
        /// </remarks>
        public override double Next()
        {
            double u = 0, v = 0;
            while (u < 1e-100)
            {
                u = Random.NextDouble();
                v = Random.NextDouble();
            }
            double stdDevFactor = Sqrt(-2.0 * Log(u)) * Sin(2.0 * PI * v);
            return distribution.Mean + distribution.StandardDeviation * stdDevFactor;
        }
    }

    public RandomGenerator Random() => new NormalRandomGenerator(this);

    public RandomGenerator Random(int seed) => new NormalRandomGenerator(seed, this);

    public RandomGenerator Random(Random? random) => new NormalRandomGenerator(random, this);

    public double Median => Mean;
    public double Variance => StandardDeviation.Sqr();
    public double Skewness => 0;

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

    public override string ToString() => $"Normal({Mean.ToStringInvariant()},{StandardDeviation.ToStringInvariant()}^2)";
}