using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;

namespace Perfolizer.Mathematics.Distributions
{
    public class MixtureDistribution : IDistribution
    {
        private readonly int n;
        [NotNull] private readonly IReadOnlyList<IDistribution> distributions;
        [NotNull] private readonly IReadOnlyList<double> weights;
        [NotNull] private readonly InverseMonotonousFunction inverseCdf;

        public double Mean { get; }
        public double Median { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }

        public MixtureDistribution([NotNull, ItemNotNull] params IDistribution[] distributions) : this(distributions, null)
        {
        }

        public MixtureDistribution([NotNull, ItemNotNull] IReadOnlyList<IDistribution> distributions,
            [CanBeNull] IReadOnlyList<double> weights = null)
        {
            Assertion.NotNullOrEmpty(nameof(distributions), distributions);
            Assertion.ItemNotNull(nameof(distributions), distributions);

            weights ??= GetDefaultWeights(distributions);
            Assertion.NotNullOrEmpty(nameof(weights), weights);
            Assertion.Equal($"{nameof(distributions)}.Length", distributions.Count, $"{nameof(weights)}.Length", weights.Count);
            Assertion.Equal($"sum({nameof(weights)})", weights.Sum(), 1);

            n = distributions.Count;
            this.distributions = distributions;
            this.weights = weights;

            inverseCdf = new InverseMonotonousFunction(Cdf);
            Median = Quantile(0.5);
            Mean = Aggregate(d => d.Mean);
            Variance = Aggregate(d => d.Variance + d.Mean.Sqr()) - Mean * Mean;
            StandardDeviation = Variance.Sqrt();
        }

        [NotNull]
        private static double[] GetDefaultWeights([CanBeNull] IReadOnlyList<IDistribution> distributions) =>
            distributions?.Select(d => 1.0 / distributions.Count).ToArray() ?? Array.Empty<double>();

        private double Aggregate(Func<IDistribution, double> func)
        {
            double result = 0;
            for (int i = 0; i < n; i++)
                result += weights[i] * func(distributions[i]);
            return result;
        }

        public double Pdf(double x) => Aggregate(d => d.Pdf(x));

        public double Cdf(double x) => Aggregate(d => d.Cdf(x));

        public double Quantile(Probability p) => inverseCdf.GetValue(p);
    }
}