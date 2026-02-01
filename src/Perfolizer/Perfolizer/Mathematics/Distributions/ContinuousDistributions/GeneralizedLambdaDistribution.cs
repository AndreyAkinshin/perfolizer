using Perfolizer.Mathematics.Randomization;
using Pragmastat;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

/// <summary>
/// Generalized lambda distribution
/// <remarks>
/// The distribution is defined by its quantile function:
/// Q(p) = mu + sigma * (p^a - (1 - p)^b)
/// </remarks>
/// </summary>
public class GeneralizedLambdaDistribution : IContinuousDistribution
{
    public double Mu { get; }
    public double Sigma { get; }
    public double A { get; }
    public double B { get; }

    public GeneralizedLambdaDistribution(double mu, double sigma, double a, double b)
    {
        Mu = mu;
        Sigma = sigma;
        A = a;
        B = b;
    }

    public double Pdf(double x) => throw new NotImplementedException();
    public double Cdf(double x) => throw new NotImplementedException();

    public double Quantile(Probability p) => Mu + Sigma * (Pow(p, A) - Pow(1 - p, B));

    public RandomGenerator Random(Rng? rng = null) => new DistributionRandomGenerator(this, rng);

    public double Mean => throw new NotImplementedException();
    public double Median => throw new NotImplementedException();
    public double Variance => throw new NotImplementedException();
    public double StandardDeviation => throw new NotImplementedException();
}