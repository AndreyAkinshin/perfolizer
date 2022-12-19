using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Range = Perfolizer.Mathematics.Common.Range;

namespace Perfolizer.Mathematics.Functions
{
    public abstract class QuantileCompareFunction
    {
        protected IQuantileEstimator QuantileEstimator { get; }

        protected QuantileCompareFunction(IQuantileEstimator? quantileEstimator = null)
        {
            QuantileEstimator = quantileEstimator ?? HarrellDavisQuantileEstimator.Instance;
        }

        public virtual double Value(Sample a, Sample b, Probability probability)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);

            double quantileA = QuantileEstimator.Quantile(a, probability);
            double quantileB = QuantileEstimator.Quantile(b, probability);
            return CalculateValue(quantileA, quantileB);
        }

        public virtual double[] Values(Sample a, Sample b, IReadOnlyList<Probability> probabilities)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);

            double[] quantilesA = QuantileEstimator.Quantiles(a, probabilities);
            double[] quantilesB = QuantileEstimator.Quantiles(b, probabilities);
            double[] values = new double[probabilities.Count];
            for (int i = 0; i < values.Length; i++)
                values[i] = CalculateValue(quantilesA[i], quantilesB[i]);
            return values;
        }

        protected abstract double CalculateValue(double quantileA, double quantileB);
        
        public Range Range(Sample a, Sample b, Probability margin, int? quantizationCount = null)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);
            Assertion.InRangeInclusive(nameof(margin), margin, 0, 0.5);
            if (quantizationCount.HasValue)
                Assertion.Positive(nameof(quantizationCount), quantizationCount.Value);

            double left = margin.Value;
            double right = 1 - left;
            int count = quantizationCount ?? (int)Math.Round((right - left) / 0.01 + 1);
            var probabilities = new Probability[count];
            if (count == 1)
                probabilities[0] = Probability.Half;
            else
                for (int i = 0; i < count; i++)
                    probabilities[i] = left + (right - left) / (count - 1) * i;
                
            double[] quantileValues = Values(a, b, probabilities);
            return Common.Range.Of(quantileValues.Min(), quantileValues.Max());
        }

        public Range Range(Sample a, Sample b)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);
            
            int n = Math.Min(a.Count, b.Count);
            double margin = Math.Min(0.5, 1 - 0.001.Pow(1.0 / n));
            return Range(a, b, margin);
        }
    }
}