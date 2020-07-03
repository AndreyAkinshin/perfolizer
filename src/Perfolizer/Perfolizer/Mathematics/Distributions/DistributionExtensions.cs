using System;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions
{
    public static class DistributionExtensions
    {
        [NotNull]
        public static RandomGenerator Random([NotNull] this IDistribution distribution)
            => new DistributionRandomGenerator(distribution);

        [NotNull]
        public static RandomGenerator Random([NotNull] this IDistribution distribution, int seed)
            => new DistributionRandomGenerator(seed, distribution);

        [NotNull]
        public static RandomGenerator Random([NotNull] this IDistribution distribution, Random random)
            => new DistributionRandomGenerator(random, distribution);
    }
}