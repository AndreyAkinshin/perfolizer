using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Randomization;
using Pragmastat;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class LaplaceDistribution : IContinuousDistribution
{
    public double Mu { get; }
    public double Sigma { get; }

    public LaplaceDistribution(double mu, double sigma)
    {
        Assertion.Positive(nameof(sigma), sigma);

        Mu = mu;
        Sigma = sigma;
    }

    public double Pdf(double x) => Exp(-Abs(x - Mu) / Sigma) / 2 / Sigma;

    public double Cdf(double x) => x <= Mu
        ? Exp((x - Mu) / Sigma) / 2
        : 1 - Exp(-(x - Mu) / Sigma) / 2;

    public double Quantile(Probability p) => p < Probability.Half
        ? Mu + Sigma * Log(2 * p)
        : Mu - Sigma * Log(2 - 2 * p);

    public RandomGenerator Random(Rng? rng = null) => new DistributionRandomGenerator(this, rng);

    public double Mean => Mu;
    public double Median => Mu;
    public double Variance => 2 * Sigma.Sqr();
    public double StandardDeviation => Constants.Sqrt2 * Sigma;
        
    public override string ToString() => $"Laplace({Mu.ToStringInvariant()},{Sigma.ToStringInvariant()})";
}