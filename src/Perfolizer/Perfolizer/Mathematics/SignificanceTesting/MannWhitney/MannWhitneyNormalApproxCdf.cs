using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Mathematics.SignificanceTesting.MannWhitney;

public class MannWhitneyNormalApproxCdf : IMannWhitneyCdf
{
    public static readonly MannWhitneyNormalApproxCdf Instance = new();

    public double Cdf(int n, int m, int u)
    {
        double mu = n * m / 2.0;
        double su = Sqrt(n * m * (n + m + 1) / 12.0);
        double z = (u - mu) / su;
        return NormalDistribution.Gauss(z);
    }
}