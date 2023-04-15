using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class ExponentialDistribution : IContinuousDistribution
{
    public double Rate { get; }

    public ExponentialDistribution(double rate = 1.0)
    {
        Assertion.Positive(nameof(rate), rate);
            
        Rate = rate;
    }

    public double Pdf(double x)
    {
        if (x < 0)
            return 0;
        return Rate * Exp(-Rate * x);
    }

    public double Cdf(double x)
    {
        if (x < 0)
            return 0;
        return 1 - Exp(-Rate * x);
    }

    public double Quantile(Probability p) => - Log(1 - p) / Rate;

    public RandomGenerator Random(Random? random = null) => new DistributionRandomGenerator(this, random);

    public double Mean => 1 / Rate;
    public double Median => Constants.Log2 / Rate;
    public double Variance => 1 / Rate.Sqr();
    public double StandardDeviation => 1 / Rate;
        
    public override string ToString() => $"Exp({Rate.ToStringInvariant()})";
}