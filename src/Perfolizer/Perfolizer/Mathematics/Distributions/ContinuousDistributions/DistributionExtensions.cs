using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public static class DistributionExtensions
{
    public static RandomGenerator Random(this IContinuousDistribution distribution, int seed)
        => distribution.Random(new Random(seed));
}