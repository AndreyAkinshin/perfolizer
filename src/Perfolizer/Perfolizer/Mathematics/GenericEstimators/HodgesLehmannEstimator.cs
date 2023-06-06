using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.GenericEstimators;

/// <summary>
/// Based on:
/// Hodges, J. L., and E. L. Lehmann. 1963. Estimates of location based on rank tests.
/// The Annals of Mathematical Statistics 34 (2):598–611.  
/// DOI: 10.1214/aoms/1177704172
///
/// The weighted version is based on https://aakinshin.net/posts/whl/
/// </summary>
public class HodgesLehmannEstimator : ILocationShiftEstimator, IMedianEstimator
{
    public static readonly HodgesLehmannEstimator Instance = new(SimpleQuantileEstimator.Instance);

    private readonly IQuantileEstimator quantileEstimator;

    public HodgesLehmannEstimator(IQuantileEstimator quantileEstimator)
    {
        this.quantileEstimator = quantileEstimator;
    }

    public double LocationShift(Sample x, Sample y)
    {
        if (!quantileEstimator.SupportsWeightedSamples)
        {
            Assertion.NonWeighted(nameof(x), x);
            Assertion.NonWeighted(nameof(y), y);
        }

        int n = x.Count, m = y.Count;
        if (x.IsWeighted || y.IsWeighted)
        {
            double[] diffs = new double[n * m];
            double[] diffsWeights = new double[n * m];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                diffs[k] = x.Values[j] - y.Values[i];
                diffsWeights[k++] = x.Weights[j] * y.Weights[i];
            }
            return quantileEstimator.Median(new Sample(diffs, diffsWeights));
        }
        else
        {
            double[] diffs = new double[n * m];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                diffs[k++] = x.Values[j] - y.Values[i];
            return quantileEstimator.Median(new Sample(diffs));
        }
    }

    public double Median(Sample x)
    {
        if (!quantileEstimator.SupportsWeightedSamples)
            Assertion.NonWeighted(nameof(x), x);

        int n = x.Count;
        if (x.IsWeighted)
        {
            double[] diffs = new double[n * (n + 1) / 2];
            double[] diffsWeights = new double[n * (n + 1) / 2];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = i; j < n; j++)
            {
                diffs[k] = (x.Values[i] + x.Values[j]) / 2;
                diffsWeights[k++] = x.Weights[i] * x.Weights[j];
            }
            return quantileEstimator.Median(new Sample(diffs, diffsWeights));
        }
        else
        {
            double[] diffs = new double[n * (n + 1) / 2];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = i; j < n; j++)
                diffs[k++] = (x.Values[i] + x.Values[j]) / 2;
            return quantileEstimator.Median(new Sample(diffs));
        }
    }
}