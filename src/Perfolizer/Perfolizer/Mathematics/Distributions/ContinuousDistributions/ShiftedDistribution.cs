using Perfolizer.Extensions;
using Perfolizer.Mathematics.Randomization;
using Pragmastat;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class ShiftedDistribution : IContinuousDistribution
{
    private readonly IContinuousDistribution distribution;
    private readonly double shift;

    public ShiftedDistribution(IContinuousDistribution distribution, double shift)
    {
        this.distribution = distribution;
        this.shift = shift;
    }

    public double Pdf(double x) => distribution.Pdf(x - shift);

    public double Cdf(double x) => distribution.Cdf(x - shift);

    public double Quantile(Probability p) => distribution.Quantile(p) + shift;

    public RandomGenerator Random(Rng? rng = null) => new ShiftedRandomGenerator(distribution.Random(rng), shift);

    public double Mean => distribution.Mean + shift;
    public double Median => distribution.Median + shift;
    public double Variance => distribution.Variance;
    public double StandardDeviation => distribution.StandardDeviation;

    public override string ToString() => $"Shifted({distribution},{shift.ToStringInvariant()})";

    private class ShiftedRandomGenerator : RandomGenerator
    {
        private readonly RandomGenerator randomGenerator;
        private readonly double shift;

        public ShiftedRandomGenerator(RandomGenerator randomGenerator, double shift)
        {
            this.randomGenerator = randomGenerator;
            this.shift = shift;
        }

        public override double Next() => randomGenerator.Next() + shift;
    }
}