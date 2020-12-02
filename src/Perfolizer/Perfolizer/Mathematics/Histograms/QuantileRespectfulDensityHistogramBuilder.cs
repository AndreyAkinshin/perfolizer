using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Sequences;

namespace Perfolizer.Mathematics.Histograms
{
    /// <summary>
    /// Empirical probability density histogram.
    ///
    /// See: https://aakinshin.net/posts/qrde-hd/
    /// </summary>
    public class QuantileRespectfulDensityHistogramBuilder : IDensityHistogramBuilder
    {
        public static readonly QuantileRespectfulDensityHistogramBuilder Instance = new QuantileRespectfulDensityHistogramBuilder();

        public DensityHistogram Build(Sample sample, int binCount) => Build(sample, binCount, null);

        [NotNull]
        public DensityHistogram Build([NotNull] Sample sample, int binCount, [CanBeNull] IQuantileEstimator quantileEstimator)
        {
            Assertion.NotNull(nameof(sample), sample);
            Assertion.MoreThan(nameof(binCount), binCount, 1);

            quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
            if (sample.IsWeighted && !quantileEstimator.SupportsWeightedSamples)
                throw new WeightedSampleNotSupportedException();

            Probability[] probabilities = Probability.ToProbabilities(
                new ArithmeticProgressionSequence(0, 1.0 / binCount).GenerateArray(binCount + 1));
            double[] quantiles = quantileEstimator.GetQuantiles(sample, probabilities);

            var bins = new List<DensityHistogramBin>(binCount);
            for (int i = 0; i < binCount; i++)
            {
                double width = quantiles[i + 1] - quantiles[i];
                double value = 1.0 / binCount / width;
                bins.Add(new DensityHistogramBin(quantiles[i], quantiles[i + 1], value));
            }

            return new DensityHistogram(bins);
        }
    }
}