using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

/// <summary>
/// Gumbel distribution (Generalized Extreme Value distribution Type-I)
/// </summary>
public class GumbelDistribution : IContinuousDistribution
{
    public double Location { get; }
    public double Scale { get; }

    private double M => Location;
    private double S => Scale;
    private double Z(double x) => (x - M) / S;

    public GumbelDistribution(double location = 0, double scale = 1)
    {
        Assertion.Positive(nameof(scale), scale);

        Location = location;
        Scale = scale;
    }

    public double Pdf(double x)
    {
        double z = Z(x);
        return Exp(-(z + Exp(-z))) / S;
    }

    public double Cdf(double x) => Exp(-Exp(-Z(x)));

    public double Quantile(Probability p)
    {
        return p.Value switch
        {
            0 => double.NegativeInfinity,
            1 => double.PositiveInfinity,
            _ => M - S * Log(-Log(p))
        };
    }
        
    public RandomGenerator Random(Random? random = null) => new DistributionRandomGenerator(this, random);

    public double Mean => M + S * Constants.EulerMascheroni;
    public double Median => M - S * Log(Log(2));
    public double Variance => PI * PI * S * S / 6;
    public double StandardDeviation => Variance.Sqrt();

    public override string ToString() => $"Gumbel({Location.ToStringInvariant()},{Scale.ToStringInvariant()})";
}