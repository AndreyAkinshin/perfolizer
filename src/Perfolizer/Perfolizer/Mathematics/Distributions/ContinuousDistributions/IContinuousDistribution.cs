using Perfolizer.Mathematics.Randomization;
using Pragmastat;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public interface IContinuousDistribution
{
    /// <summary>
    /// Probability density function
    /// </summary>
    double Pdf(double x);

    /// <summary>
    /// Cumulative distribution function
    /// </summary>
    double Cdf(double x);

    /// <summary>
    /// Quantile function
    /// </summary>
    double Quantile(Probability p);

    RandomGenerator Random(Rng? rng = null);

    double Mean { get; }
    double Median { get; }
    double Variance { get; }
    double StandardDeviation { get; }
}