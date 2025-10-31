using System.Text;
using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.Randomization;
using Pragmastat;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class MixtureDistribution : IContinuousDistribution
{
    private readonly int n;
    private readonly IReadOnlyList<IContinuousDistribution> distributions;
    private readonly IReadOnlyList<double> weights;
    private readonly InverseMonotonousFunction inverseCdf;
    private readonly Lazy<string> lazyToString;

    public double Mean { get; }
    public double Median { get; }
    public double Variance { get; }
    public double StandardDeviation { get; }

    public MixtureDistribution(params IContinuousDistribution[] distributions) : this(distributions, null)
    {
    }

    public MixtureDistribution(IReadOnlyList<IContinuousDistribution> distributions,
        IReadOnlyList<double>? weights = null)
    {
        Assertion.NotNullOrEmpty(nameof(distributions), distributions);
        Assertion.ItemNotNull(nameof(distributions), distributions);

        bool isWeighted = weights != null;
        weights ??= GetDefaultWeights(distributions);
        Assertion.NotNullOrEmpty(nameof(weights), weights);
        Assertion.Equal($"{nameof(distributions)}.Length", distributions.Count, $"{nameof(weights)}.Length", weights.Count);
        Assertion.Positive(nameof(weights), weights);

        double totalWeight = weights.Sum();
        if (Math.Abs(totalWeight - 1) < 1e-9)
            weights = weights.Select(w => w / totalWeight).ToArray();

        n = distributions.Count;
        this.distributions = distributions;
        this.weights = weights;

        double cdfMin = distributions.Select(d => d.Quantile(0)).Min();
        if (double.IsInfinity(cdfMin))
            cdfMin = double.MinValue / 3;
        double cdfMax = distributions.Select(d => d.Quantile(1)).Max();
        if (double.IsInfinity(cdfMax))
            cdfMax = double.MaxValue / 3;
        inverseCdf = new InverseMonotonousFunction(Cdf, cdfMin, cdfMax);
        Median = Quantile(0.5);
        Mean = Aggregate(d => d.Mean);
        Variance = Aggregate(d => d.Variance + d.Mean.Sqr()) - Mean * Mean;
        StandardDeviation = Variance.Sqrt();

        lazyToString = new Lazy<string>(() =>
        {
            var builder = new StringBuilder();
            builder.Append("Mix(");
            for (int i = 0; i < distributions.Count; i++)
            {
                if (i != 0)
                    builder.Append(";");
                builder.Append(distributions[i]);
                if (isWeighted)
                {
                    builder.Append("|");
                    builder.Append(weights[i].ToStringInvariant());
                }
            }
            builder.Append(")");
            return builder.ToString();
        });
    }

    private static double[] GetDefaultWeights(IReadOnlyList<IContinuousDistribution>? distributions) =>
        distributions?.Select(_ => 1.0 / distributions.Count).ToArray() ?? Array.Empty<double>();

    private double Aggregate(Func<IContinuousDistribution, double> func)
    {
        double result = 0;
        for (int i = 0; i < n; i++)
            result += weights[i] * func(distributions[i]);
        return result;
    }

    public double Pdf(double x) => Aggregate(d => d.Pdf(x));

    public double Cdf(double x) => Aggregate(d => d.Cdf(x));

    public double Quantile(Probability p) => inverseCdf.Value(p);

    public RandomGenerator Random(Random? random = null) => new MixtureRandomGenerator(this, random ?? new Random());

    public override string ToString() => lazyToString.Value;

    private class MixtureRandomGenerator : RandomGenerator
    {
        private readonly MixtureDistribution mixture;
        private readonly RandomGenerator[] generators;

        public MixtureRandomGenerator(MixtureDistribution mixture, Random random) : base(random)
        {
            this.mixture = mixture;
            generators = mixture.distributions.Select(d => d.Random(Random)).ToArray();
        }

        public override double Next()
        {
            double value = Random.NextDouble();
            double sum = 0;
            for (int i = 0; i < mixture.n; i++)
            {
                sum += mixture.weights[i];
                if (value < sum)
                    return generators[i].Next();
            }
            return generators.Last().Next();
        }
    }
}