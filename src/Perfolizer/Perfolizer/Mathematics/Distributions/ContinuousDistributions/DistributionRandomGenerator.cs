using Perfolizer.Mathematics.Randomization;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class DistributionRandomGenerator : RandomGenerator
{
    private readonly IContinuousDistribution distribution;

    public DistributionRandomGenerator(IContinuousDistribution distribution)
    {
        this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
    }

    public DistributionRandomGenerator(IContinuousDistribution distribution, long seed) : base(seed)
    {
        this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
    }

    public DistributionRandomGenerator(IContinuousDistribution distribution, Rng? rng = null) : base(rng ?? new Rng())
    {
        this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
    }

    public override double Next() => distribution.Quantile(Rng.UniformDouble());
}