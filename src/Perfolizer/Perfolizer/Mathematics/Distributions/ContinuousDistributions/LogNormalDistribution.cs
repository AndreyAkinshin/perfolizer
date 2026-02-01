using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.Randomization;
using Pragmastat;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

/// <summary>
/// Log-normal distribution (Galton's distribution)
/// </summary>
public class LogNormalDistribution : IContinuousDistribution
{
    public double Mean { get; }
    public double StandardDeviation { get; }

    public LogNormalDistribution(double mean = 0, double stdDev = 1)
    {
        Assertion.Positive(nameof(stdDev), stdDev);

        Mean = mean;
        StandardDeviation = stdDev;
    }
        
    public double Pdf(double x)
    {
        if (x < 1e-9)
            return 0;
        return Exp(-(Log(x) - Mean).Sqr() / (2 * StandardDeviation.Sqr())) / (x * StandardDeviation * Constants.Sqrt2Pi);
    }

    public double Cdf(double x)
    {
        if (x < 1e-9)
            return 0;
        return 0.5 * (1 + ErrorFunction.Value((Log(x) - Mean) / (Constants.Sqrt2 * StandardDeviation)));
    }

    public double Quantile(Probability p)
    {
        if (p < 1e-9)
            return 0;
        if (p > 1 - 1e-9)
            return double.PositiveInfinity;
        return Exp(Mean + Constants.Sqrt2 * StandardDeviation * ErrorFunction.InverseValue(2 * p - 1));
    }

    public RandomGenerator Random(Rng? rng = null)
    {
        var normalDistribution = new NormalDistribution(Mean, StandardDeviation);
        var normalRandomGenerator = new NormalDistribution.NormalRandomGenerator(rng, normalDistribution);
        return new LogNormalRandomGenerator(normalRandomGenerator);
    }

    public double Median => Exp(Mean);
    public double Variance => StandardDeviation.Sqr();

    private class LogNormalRandomGenerator : RandomGenerator
    {
        private readonly NormalDistribution.NormalRandomGenerator normalRandomGenerator;

        public LogNormalRandomGenerator(NormalDistribution.NormalRandomGenerator normalRandomGenerator)
        {
            this.normalRandomGenerator = normalRandomGenerator;
        }

        public override double Next() => Exp(normalRandomGenerator.Next());
    }
        
    public override string ToString() => $"LogNormal({Mean.ToStringInvariant()},{StandardDeviation.ToStringInvariant()}^2)";        
}