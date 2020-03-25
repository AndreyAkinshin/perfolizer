using System;
using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions
{
    public class UniformDistribution
    {
        /// <summary>
        /// The minimum value of the uniform distribution
        /// </summary>
        public double Min { get; }

        /// <summary>
        /// The maximum value of the uniform distribution
        /// </summary>
        public double Max { get; }

        public UniformDistribution(double min, double max)
        {
            if (min >= max)
                throw new ArgumentOutOfRangeException(nameof(min), $"{nameof(min)} should be less than {nameof(max)}");

            Min = min;
            Max = max;
        }

        /// <summary>
        /// Probability density function 
        /// </summary>
        public double Pdf(double x) => x < Min || x > Max ? 0 : 1 / (Max - Min);

        /// <summary>
        /// Cumulative distribution function
        /// </summary>
        public double Cdf(double x)
        {
            if (x < Min)
                return 0;
            if (x > Max)
                return 1;
            return (x - Min) / (Max - Min);
        }

        /// <summary>
        /// Quantile function
        /// </summary>
        public double Quantile(double q)
        {
            if (q < 0 || q > 1)
                throw new ArgumentOutOfRangeException(nameof(q), $"{nameof(q)} should be inside [0;1]");
            return Min + q * (Max - Min);
        }

        private class UniformRandomGenerator : RandomGenerator
        {
            private readonly UniformDistribution distribution;

            public UniformRandomGenerator(UniformDistribution distribution)
            {
                this.distribution = distribution;
            }

            public UniformRandomGenerator(int seed, UniformDistribution distribution) : base(seed)
            {
                this.distribution = distribution;
            }

            public UniformRandomGenerator(Random random, UniformDistribution distribution) : base(random)
            {
                this.distribution = distribution;
            }

            public override double Next()
            {
                return distribution.Min + (distribution.Max - distribution.Min) * Random.NextDouble();
            }
        }

        [NotNull]
        public RandomGenerator Random() => new UniformRandomGenerator(this);

        [NotNull]
        public RandomGenerator Random(int seed) => new UniformRandomGenerator(seed, this);

        [NotNull]
        public RandomGenerator Random(Random random) => new UniformRandomGenerator(random, this);

        public double Mean => (Min + Max) / 2;
        public double Median => (Min + Max) / 2;
        public double Variance => (Max - Min).Sqr() / 12;
        public double StdDev => Math.Sqrt(Variance);
        public double Skewness => 0;

        [NotNull]
        public string ToString([CanBeNull] CultureInfo cultureInfo, [CanBeNull] string format)
        {
            cultureInfo ??= DefaultCultureInfo.Instance;
            return format == null
                ? $"unif({Min.ToString(cultureInfo)}, {Max.ToString(cultureInfo)})"
                : $"unif({Min.ToString(format, cultureInfo)}, {Max.ToString(format, cultureInfo)})";
        }

        [NotNull]
        public string ToString([NotNull] string format) => ToString(null, format);

        [NotNull]
        public override string ToString() => ToString(null, null);
    }
}