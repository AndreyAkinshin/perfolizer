using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.Distributions.DiscreteDistributions;

public interface IDiscreteDistribution
{
    /// <summary>
    /// Probability mass function 
    /// </summary>
    double Pmf(int k);

    /// <summary>
    /// Cumulative distribution function
    /// </summary>
    double Cdf(int k);

    /// <summary>
    /// Quantile function
    /// </summary>
    int Quantile(Probability p);
}