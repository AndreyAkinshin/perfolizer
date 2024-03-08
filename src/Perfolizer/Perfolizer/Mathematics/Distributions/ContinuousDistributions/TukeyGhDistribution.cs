using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

/// <summary>
/// Tukey g-and-h distribution
/// </summary>
public class TukeyGhDistribution : IContinuousDistribution
{
    public double Mu { get; }
    public double Sigma { get; }
    public double G { get; }
    public double H { get; }

    public TukeyGhDistribution(double mu, double sigma, double g, double h)
    {
        Mu = mu;
        Sigma = sigma;
        G = g;
        H = h;
    }

    public double Pdf(double x) => throw new NotImplementedException();

    public double Cdf(double x) => throw new NotImplementedException();

    public double Quantile(Probability p)
    {
        if (Abs(p) < 1e-9)
            return double.NegativeInfinity;
        if (Abs(p) > 1 - 1e-9)
            return double.PositiveInfinity;
        double z = NormalDistribution.Standard.Quantile(p);
        return Abs(G) < 1e-9
            ? Mu + Sigma * z * Exp(H * z * z / 2)
            : Mu + Sigma * (Exp(G * z) - 1) * Exp(H * z * z / 2) / G;
    }

    public RandomGenerator Random(Random? random = null) => new DistributionRandomGenerator(this, random);

    public double Mean => throw new NotImplementedException();
    public double Median => throw new NotImplementedException();
    public double Variance => throw new NotImplementedException();
    public double StandardDeviation => throw new NotImplementedException();

    public override string ToString() =>
        $"TukeyGH({Mu.ToStringInvariant()},{Sigma.ToStringInvariant()},{G.ToStringInvariant()},{H.ToStringInvariant()})";
}