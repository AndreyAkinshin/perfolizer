using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Sequences;
using Pragmastat;

namespace Perfolizer.Mathematics.Histograms;

/// <summary>
/// Empirical probability density histogram.
///
/// See: https://aakinshin.net/posts/qrde-hd/
/// </summary>
public class QuantileRespectfulDensityHistogramBuilder : IDensityHistogramBuilder
{
    public static readonly QuantileRespectfulDensityHistogramBuilder Instance = new QuantileRespectfulDensityHistogramBuilder();

    public DensityHistogram Build(Sample sample, int binCount) => Build(sample, binCount, null);

    public DensityHistogram Build(Sample sample, int binCount, IQuantileEstimator? quantileEstimator)
    {
        Assertion.NotNull(nameof(sample), sample);
        Assertion.MoreThan(nameof(binCount), binCount, 1);

        quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
        if (sample.IsWeighted && !quantileEstimator.SupportsWeightedSamples)
            throw new WeightedSampleNotSupportedException();

        double[] probabilityValues = new ArithmeticProgressionSequence(0, 1.0 / binCount).GenerateArray(binCount + 1);
        var probabilities = Probability.ToProbabilities(probabilityValues);
        double[] quantiles = quantileEstimator.Quantiles(sample, probabilities);

        var bins = new List<DensityHistogramBin>(binCount);
        for (int i = 0; i < binCount; i++)
        {
            double width = quantiles[i + 1] - quantiles[i];
            if (width > 1e-9)
            {
                double value = 1.0 / binCount / width;
                bins.Add(new DensityHistogramBin(quantiles[i], quantiles[i + 1], value));
            }
        }

        return new DensityHistogram(bins);
    }
}