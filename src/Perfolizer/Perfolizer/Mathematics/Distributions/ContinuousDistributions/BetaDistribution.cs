using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class BetaDistribution : IContinuousDistribution
{
    public double Alpha { get; }
    public double Beta { get; }

    private readonly Lazy<double> lazyMedian;

    public BetaDistribution(double alpha, double beta)
    {
        Assertion.NonNegative(nameof(alpha), alpha);
        Assertion.NonNegative(nameof(beta), beta);

        Alpha = alpha;
        Beta = beta;
        lazyMedian = new Lazy<double>(() => Quantile(0.5));
    }

    /// <summary>
    /// Probability density function 
    /// </summary>
    public double Pdf(double x)
    {
        if (x < 0 || x > 1)
            return 0;

        if (x < 1e-9)
        {
            if (Alpha > 1)
                return 0;
            if (Abs(Alpha - 1) < 1e-9)
                return Beta;
            return double.PositiveInfinity;
        }

        if (x > 1 - 1e-9)
        {
            if (Beta > 1)
                return 0;
            if (Abs(Beta - 1) < 1e-9)
                return Alpha;
            return double.PositiveInfinity;
        }

        if (Alpha < 1e-9 || Beta < 1e-9)
            return 0;

        return Exp((Alpha - 1) * Log(x) + (Beta - 1) * Log(1 - x) - BetaFunction.CompleteLogValue(Alpha, Beta));
    }

    /// <summary>
    /// Cumulative distribution function
    /// </summary>
    public double Cdf(double x) => BetaFunction.RegularizedIncompleteValue(Alpha, Beta, x);

    public double Quantile(Probability p) => BetaFunction.RegularizedIncompleteInverseValue(Alpha, Beta, p);

    public RandomGenerator Random(Random? random = null) => new DistributionRandomGenerator(this, random);

    public double Mean => Alpha / (Alpha + Beta);
    public double Median => lazyMedian.Value;
    public double Variance => Alpha * Beta / (Alpha + Beta).Sqr() / (Alpha + Beta + 1);
    public double StandardDeviation => Variance.Sqrt();
    public double Skewness => 2 * (Beta - Alpha) * Sqrt(Alpha + Beta + 1) / (Alpha + Beta + 2) / Sqrt(Alpha * Beta);

    public override string ToString() => $"Beta({Alpha.ToStringInvariant()},{Beta.ToStringInvariant()})";
}