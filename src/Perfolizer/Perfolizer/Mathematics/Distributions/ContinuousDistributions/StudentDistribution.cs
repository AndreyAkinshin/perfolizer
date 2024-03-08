using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class StudentDistribution : IContinuousDistribution
{
    private readonly double df;

    public StudentDistribution(double df)
    {
        this.df = df;
    }

    public double Pdf(double x)
    {
        double df2 = (df + 1) / 2;
            
        // Ln( Г((df + 1) / 2) / Г(df / 2) )
        double term1 = GammaFunction.LogValue(df2) - GammaFunction.LogValue(df / 2);

        // Ln( (1 + x^2 / df) ^ (-(df + 1) / 2) )
        double term2 = Log(1 + x.Sqr() / df) * -df2;
            
        return Exp(term1 + term2) / Sqrt(PI * df);
    }

    public double Cdf(double x)
    {
        double p = 0.5 * BetaFunction.RegularizedIncompleteValue(0.5 * df, 0.5, df / (df + x.Sqr()));
        return x > 0 ? 1 - p : p;
    }

    public double Quantile(Probability p)
    {
        double x = BetaFunction.RegularizedIncompleteInverseValue(0.5 * df, 0.5, 2 * Min(p, 1 - p));
        x = Sqrt(df * (1 - x) / x);
        return p >= 0.5 ? x : -x;
    }
        
    public RandomGenerator Random(Random? random = null) => new DistributionRandomGenerator(this, random);

    public double Mean => df > 1 ? 0 : double.NaN;

    public double Median => 0;
    public double Variance => df > 2 
        ? df / (df - 2) 
        : df > 1 ? double.PositiveInfinity : double.NaN;

    public double StandardDeviation => Variance.Sqrt();

    public override string ToString() => $"Student({df.ToStringInvariant()})";
}