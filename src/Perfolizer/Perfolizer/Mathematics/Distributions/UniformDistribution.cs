using System;
using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.Distributions
{
    public class UniformDistribution : IDistribution
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
                throw new ArgumentOutOfRangeException(nameof(min), min, $"{nameof(min)} should be less than {nameof(max)}");

            Min = min;
            Max = max;
        }

        public double Pdf(double x) => x < Min || x > Max ? 0 : 1 / (Max - Min);

        public double Cdf(double x)
        {
            if (x < Min)
                return 0;
            if (x > Max)
                return 1;
            return (x - Min) / (Max - Min);
        }

        public double Quantile(double q)
        {
            if (q < 0 || q > 1)
                throw ExceptionHelper.RangeRequirementException(nameof(q), q, 0, 1);
            return Min + q * (Max - Min);
        }

        public double Mean => (Min + Max) / 2;
        public double Median => (Min + Max) / 2;
        public double Variance => (Max - Min).Sqr() / 12;
        public double StandardDeviation => Math.Sqrt(Variance);
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