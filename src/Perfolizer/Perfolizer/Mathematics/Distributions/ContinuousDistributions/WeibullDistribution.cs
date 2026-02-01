using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.Randomization;
using Pragmastat;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class WeibullDistribution : IContinuousDistribution
{
    public double Scale { get; }
    public double Shape { get; }

    private double Lambda => Scale;
    private double K => Shape;

    public WeibullDistribution(double shape, double scale = 1)
    {
        Assertion.Positive(nameof(shape), shape);
        Assertion.Positive(nameof(scale), scale);

        Scale = scale;
        Shape = shape;
    }

    public double Pdf(double x) => x < 0 ? 0 : K / Lambda * Pow(x / Lambda, K - 1) * Exp(-Pow(x / Lambda, K));

    public double Cdf(double x) => x < 0 ? 0 : 1 - Exp(-Pow(x / Lambda, K));

    public double Quantile(Probability p)
    {
        return p.Value switch
        {
            0 => 0,
            1 => double.PositiveInfinity,
            _ => Lambda * Pow(-Log(1 - p), 1 / K)
        };
    }
        
    public RandomGenerator Random(Rng? rng = null) => new DistributionRandomGenerator(this, rng);

    public double Mean => Lambda * GammaFunction.Value(1 + 1 / K);
    public double Median => Lambda * Pow(Constants.Log2, 1 / K);
    public double Variance => Lambda.Sqr() * (GammaFunction.Value(1 + 2 / K) - GammaFunction.Value(1 + 1 / K).Sqr());
    public double StandardDeviation => Variance.Sqrt();

    public override string ToString() => $"Weibull({Scale.ToStringInvariant()},{Shape.ToStringInvariant()})";
}