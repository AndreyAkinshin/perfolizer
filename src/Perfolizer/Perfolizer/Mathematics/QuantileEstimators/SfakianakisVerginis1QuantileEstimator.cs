using Perfolizer.Metrology;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Mathematics.QuantileEstimators;

/// <summary>
/// SV1 quantile estimator
/// <remarks>
/// Based on the following paper:
/// Sfakianakis, Michael E., and Dimitris G. Verginis. "A new family of nonparametric quantile estimators."
/// Communications in Statistics—Simulation and Computation® 37, no. 2 (2008): 337-345.
/// https://doi.org/10.1080/03610910701790491
/// </remarks>
/// </summary>
public class SfakianakisVerginis1QuantileEstimator : BinomialBasedQuantileEstimator
{
    public static readonly IQuantileEstimator Instance = new SfakianakisVerginis1QuantileEstimator();

    private SfakianakisVerginis1QuantileEstimator()
    {
    }

    public override string Alias => "SV1";

    protected override Measurement Quantile(Sample sample, Probability probability, double[] b)
    {
        var x = sample.SortedValues;
        int n = x.Count;
        double value = 0;
        value += (2 * b[0] + b[1]) * x[0] / 2 + b[0] * x[1] / 2 - b[0] * x[2] / 2;
        for (int i = 2; i <= n - 1; i++)
            value += (b[i] + b[i - 1]) / 2 * x[i - 1];
        value += -b[n] * x[n - 3] / 2 + b[n] * x[n - 2] / 2 + (2 * b[n] + b[n - 1]) * x[n - 1] / 2;
        return value.WithUnitOf(sample);
    }
}