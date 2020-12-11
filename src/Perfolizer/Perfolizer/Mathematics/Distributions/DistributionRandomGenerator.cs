using System;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions
{
    public class DistributionRandomGenerator : RandomGenerator
    {
        private readonly IDistribution distribution;

        public DistributionRandomGenerator([NotNull] IDistribution distribution)
        {
            this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        }

        public DistributionRandomGenerator([NotNull] IDistribution distribution, int seed) : base(seed)
        {
            this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        }

        public DistributionRandomGenerator([NotNull] IDistribution distribution, [CanBeNull] Random random = null) : base(random ?? new Random())
        {
            this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        }

        public override double Next() => distribution.Quantile(Random.NextDouble());
    }
}