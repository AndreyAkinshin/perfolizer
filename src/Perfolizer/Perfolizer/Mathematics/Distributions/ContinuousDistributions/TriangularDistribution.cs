using System;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Randomization;
using static System.Math;

namespace Perfolizer.Mathematics.Distributions.ContinuousDistributions;

public class TriangularDistribution: IContinuousDistribution
{
    /// <summary>
    /// The minimum value of the triangular distribution
    /// </summary>
    public double Min { get; }

    /// <summary>
    /// The maximum value of the triangular distribution
    /// </summary>
    public double Max { get; }
        
    /// <summary>
    /// The mode value of the triangular distribution
    /// </summary>
    public double Mode { get; }

    private double A => Min;
    private double B => Max;
    private double C => Mode;

    public TriangularDistribution(double min, double max, double mode)
    {
        if (min >= max)
            throw new ArgumentOutOfRangeException(nameof(min), min, $"{nameof(min)} should be less than {nameof(max)}");
        if (min > mode)
            throw new ArgumentOutOfRangeException(nameof(mode), mode, $"{nameof(mode)} should be greater than {nameof(min)}");
        if (max < mode)
            throw new ArgumentOutOfRangeException(nameof(mode), mode, $"{nameof(mode)} should be less than {nameof(max)}");

        Min = min;
        Max = max;
        Mode = mode;
    }

    public double Pdf(double x)
    {
        if (x < A || x > B)
            return 0;
        if (x < C)
            return 2 * (x - A) / (B - A) / (C - A);
        if (x > C)
            return 2 * (B - x) / (B - A) / (B - C);
        return 2 / (B - A); // x == C
    }

    public double Cdf(double x)
    {
        if (x < A)
            return 0;
        if (x <= C)
            return (x - A).Sqr() / (B - A) / (C - A);
        if (x < B)
            return 1 - (B - x).Sqr() / (B - A) / (B - C);
        return 1; // x >= B
    }

    public double Quantile(Probability p)
    {
        return p < Cdf(C)
            ? A + Sqrt(p * (B - A) * (C - A))
            : B - Sqrt((1 - p) * (B - A) * (B - C));
    }

    public RandomGenerator Random(Random? random = null) => new DistributionRandomGenerator(this, random);

    public double Mean => (Min + Mode + Max) / 3;
    public double Median => C >= (A + B) / 2
        ? A + ((B - A) * (C - A) / 2).Sqrt()
        : B - ((B - A) * (B - C) / 2).Sqrt();
    public double Variance => (A.Sqr() + B.Sqr() + C.Sqr() - A * B - A * C - B * C) / 18;
    public double StandardDeviation => Sqrt(Variance);

    public override string ToString() => $"Tri({A.ToStringInvariant()},{C.ToStringInvariant()},{B.ToStringInvariant()})";
}