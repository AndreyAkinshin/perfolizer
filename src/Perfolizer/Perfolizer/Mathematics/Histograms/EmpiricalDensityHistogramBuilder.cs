using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Sequences;

namespace Perfolizer.Mathematics.Histograms
{
    /// <summary>
    /// Empirical probability density histogram.
    ///
    /// See: https://aakinshin.net/posts/epdf-hd/
    /// </summary>
    public class EmpiricalDensityHistogramBuilder : IDensityHistogramBuilder
    {
        public const int DefaultBinCount = 100;

        public static readonly EmpiricalDensityHistogramBuilder Instance = new EmpiricalDensityHistogramBuilder();

        public DensityHistogram Build(IReadOnlyList<double> values) => Build(values, null, DefaultBinCount);

        public DensityHistogram Build(IReadOnlyList<double> values, IReadOnlyList<double> weights) =>
            Build(values, weights, DefaultBinCount);

        [NotNull]
        public DensityHistogram Build([NotNull] IReadOnlyList<double> values,
            [CanBeNull] IReadOnlyList<double> weights,
            int binCount,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);
            Assertion.MoreThan(nameof(binCount), binCount, 1);

            quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
            if (weights != null && !(quantileEstimator is IWeightedQuantileEstimator))
                throw new ArgumentException(
                    $"When {nameof(weights)} != null, {nameof(quantileEstimator)} should implement {nameof(IWeightedQuantileEstimator)}");

            var sortedValues = values.ToSorted();
            var probabilities = new ArithmeticProgressionSequence(0, 1.0 / binCount).GenerateArray(binCount + 1);
            var quantiles = quantileEstimator is IWeightedQuantileEstimator weightedQuantileEstimator && weights != null
                ? weightedQuantileEstimator.GetWeightedQuantiles(sortedValues, weights, probabilities)
                : quantileEstimator.GetQuantiles(sortedValues, probabilities);

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