using System.Diagnostics.CodeAnalysis;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Functions;

namespace Perfolizer.Mathematics.SignificanceTesting.MannWhitney;

/// <summary>
/// https://aakinshin.net/posts/mw-edgeworth2/
/// </summary>
public class MannWhitneyEdgeworthApproxCdf : IMannWhitneyCdf
{
    public static readonly MannWhitneyEdgeworthApproxCdf Instance = new();

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public double Cdf(int n, int m, int u)
    {
        double mu = n * m / 2.0;
        double su = Sqrt(n * m * (n + m + 1) / 12.0);
        double z = (u - mu - 0.5) / su;
        double phi = NormalDistribution.Standard.Pdf(z);
        double Phi = NormalDistribution.Standard.Cdf(z);

        double mu2 = n * m * (n + m + 1) / 12.0;
        double mu4 =
            n * m * (n + m + 1) *
            (0
             + 5 * m * n * (m + n)
             - 2 * (m.Pow(2) + n.Pow(2))
             + 3 * m * n
             - 2 * (n + m)
            ) / 240.0;

        double mu6 =
            n * m * (n + m + 1) *
            (0
             + 35 * m.Pow(2) * n.Pow(2) * (m.Pow(2) + n.Pow(2))
             + 70 * m.Pow(3) * n.Pow(3)
             - 42 * m * n * (m.Pow(3) + n.Pow(3))
             - 14 * m.Pow(2) * n.Pow(2) * (n + m)
             + 16 * (n.Pow(4) + m.Pow(4))
             - 52 * n * m * (n.Pow(2) + m.Pow(2))
             - 43 * n.Pow(2) * m.Pow(2)
             + 32 * (m.Pow(3) + n.Pow(3))
             + 14 * m * n * (n + m)
             + 8 * (n.Pow(2) + m.Pow(2))
             + 16 * n * m
             - 8 * (n + m)
            ) / 4032.0;

        double e3 = (mu4 / Pow(mu2, 2) - 3) / Factorial(4);
        double e5 = (mu6 / Pow(mu2, 3) - 15 * mu4 / Pow(mu2, 2) + 30) / Factorial(6);
        double e7 = 35 * Pow((mu4 / Pow(mu2, 2) - 3), 2) / Factorial(8);

        double f3 = -phi * H3(z);
        double f5 = -phi * H5(z);
        double f7 = -phi * H7(z);

        double edgeworth = Phi + e3 * f3 + e5 * f5 + e7 * f7;
        return Min(Max(edgeworth, 0), 1);

        double H3(double x) => x.Pow(3) - 3 * x;
        double H5(double x) => x.Pow(5) - 10 * x.Pow(3) + 15 * x;
        double H7(double x) => x.Pow(7) - 21 * x.Pow(5) + 105 * x.Pow(3) - 105 * x;
        double Factorial(int x) => FactorialFunction.Value(x);
    }
}