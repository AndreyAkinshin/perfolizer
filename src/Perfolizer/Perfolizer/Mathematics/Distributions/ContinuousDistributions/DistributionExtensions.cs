using System;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions
{
    public static class DistributionExtensions
    {
        [NotNull]
        public static RandomGenerator Random([NotNull] this IContinuousDistribution distribution, int seed)
            => distribution.Random(new Random(seed));
    }
}