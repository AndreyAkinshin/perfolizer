using Perfolizer.Mathematics.Randomization;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public static class DistributionExtensions
{
    public static RandomGenerator Random(this IContinuousDistribution distribution, long seed)
        => distribution.Random(new Rng(seed));
}