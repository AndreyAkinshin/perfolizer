using System;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions
{
    public class DistributionRandomGenerator : RandomGenerator
    {
        private readonly IContinuousDistribution distribution;

        public DistributionRandomGenerator([NotNull] IContinuousDistribution distribution)
        {
            this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        }

        public DistributionRandomGenerator([NotNull] IContinuousDistribution distribution, int seed) : base(seed)
        {
            this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        }

        public DistributionRandomGenerator([NotNull] IContinuousDistribution distribution, [CanBeNull] Random random = null) : base(random ?? new Random())
        {
            this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        }

        public override double Next() => distribution.Quantile(Random.NextDouble());
    }
}