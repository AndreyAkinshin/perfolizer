using System;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;

namespace Perfolizer.Mathematics.Distributions.DiscreteDistributions;

public class BinomialDistribution : IDiscreteDistribution
{
    public int N { get; }
    public Probability P { get; }

    public BinomialDistribution(int n, Probability p)
    {
        Assertion.Positive(nameof(n), n);

        N = n;
        P = p;
    }

    public double Pmf(int k)
    {
        if (k < 0 || k > N)
            return 0;

        return Math.Exp(
            BinomialCoefficientHelper.LogBinomialCoefficient(N, k) +
            k * Math.Log(P) +
            (N - k) * Math.Log(1 - P)
        );
    }

    public double Cdf(int k)
    {
        if (k < 0)
            return 0;
        if (k > N)
            return 1;
        return BetaFunction.RegularizedIncompleteValue(N - k, 1 + k, 1 - P);
    }

    public int Quantile(Probability p)
    {
        int k;
        double cdf = 0;
        for (k = 0; k < N; k++)
        {
            cdf += Pmf(k);
            if (cdf + 1e-9 >= p)
                break;
        }
        return k;
    }
}