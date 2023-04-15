using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators;

/// <summary>
/// SV2 quantile estimator
/// <remarks>
/// Based on the following paper:
/// Sfakianakis, Michael E., and Dimitris G. Verginis. "A new family of nonparametric quantile estimators."
/// Communications in Statistics—Simulation and Computation® 37, no. 2 (2008): 337-345.
/// https://doi.org/10.1080/03610910701790491
/// </remarks>
/// </summary>
public class SfakianakisVerginis2QuantileEstimator : BinomialBasedQuantileEstimator
{
    public static readonly IQuantileEstimator Instance = new SfakianakisVerginis2QuantileEstimator();

    private SfakianakisVerginis2QuantileEstimator()
    {
    }

    public override string Alias => "SV2";

    protected override double Quantile(IReadOnlyList<double> x, Probability probability, double[] b)
    {
        int n = x.Count;
        double value = 0;
        for (int i = 0; i < n; i++)
            value += b[i] * x[i];
        value += (2 * x[n - 1] - x[n - 2]) * b[n];
        return value;
    }
}