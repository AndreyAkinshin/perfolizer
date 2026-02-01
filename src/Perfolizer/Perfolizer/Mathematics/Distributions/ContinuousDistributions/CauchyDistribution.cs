using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Randomization;
using Pragmastat;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class CauchyDistribution : IContinuousDistribution
{
    public double Location { get; }
    public double Scale { get; }

    public CauchyDistribution(double location = 0, double scale = 1)
    {
        Assertion.Positive(nameof(scale), scale);
            
        Location = location;
        Scale = scale;
    }

    public double Pdf(double x)
    {
        double z = (x - Location) / Scale;
        return 1 / (PI * Scale * (1 + z.Sqr()));
    }

    public double Cdf(double x)
    {
        double z = (x - Location) / Scale;
        return Atan(z) / PI + 0.5;
    }

    public double Quantile(Probability p)
    {
        return p.Value switch
        {
            0 => double.NegativeInfinity,
            1 => double.PositiveInfinity,
            _ => Location + Scale * Tan(PI * (p - 0.5))
        };
    }

    public RandomGenerator Random(Rng? rng = null) => new DistributionRandomGenerator(this, rng);

    public double Mean => double.NaN;
    public double Median => Location;
    public double Variance => double.NaN;
    public double StandardDeviation => double.NaN;

    public override string ToString() => $"Cauchy({Location.ToStringInvariant()},{Scale.ToStringInvariant()})";
}