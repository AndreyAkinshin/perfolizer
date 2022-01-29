using System;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Randomization;
using static System.Math;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions
{
    public class ParetoDistribution : IContinuousDistribution
    {
        public double Xm { get; }
        public double Alpha { get; }

        public ParetoDistribution(double xm, double alpha)
        {
            Assertion.Positive(nameof(xm), xm);
            Assertion.Positive(nameof(alpha), alpha);

            Xm = xm;
            Alpha = alpha;
        }

        public double Pdf(double x)
        {
            if (x <= Xm)
                return 0;
            return Alpha * Pow(Xm, Alpha) / Pow(x, Alpha + 1);
        }

        public double Cdf(double x)
        {
            if (x <= Xm)
                return 0;
            return 1 - Pow(Xm / x, Alpha);
        }

        public double Quantile(Probability p) => Xm * Pow(1 - p, -1 / Alpha);
        
        public RandomGenerator Random(Random? random = null) => new DistributionRandomGenerator(this, random);

        public double Mean => Alpha <= 1 ? double.PositiveInfinity : Alpha * Xm / (Alpha - 1);
        public double Median => Xm * Pow(2, 1 / Alpha);
        public double Variance => Alpha <= 2 ? double.PositiveInfinity : Xm.Sqr() * Alpha / (Alpha - 1).Sqr() / (Alpha - 2);
        public double StandardDeviation => Variance.Sqrt();

        public override string ToString() => $"Pareto({Xm.ToStringInvariant()},{Alpha.ToStringInvariant()})";
    }
}