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

        public DistributionRandomGenerator(int seed, [NotNull] IDistribution distribution) : base(seed)
        {
            this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        }

        public DistributionRandomGenerator([NotNull] Random random, [NotNull] IDistribution distribution) : base(random)
        {
            this.distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
        }

        public override double Next() => distribution.Quantile(Random.NextDouble());
    }
}