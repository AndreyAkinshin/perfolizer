using System;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// This quantile estimator supports nine popular estimation algorithms that are described in [Hyndman1996].
    /// <remarks>
    /// Hyndman, Rob J., and Yanan Fan. "Sample quantiles in statistical packages." The American Statistician 50, no. 4 (1996): 361-365.
    /// https://doi.org/10.2307/2684934
    /// </remarks>
    /// </summary>
    public class HyndmanFanQuantileEstimator : IQuantileEstimator
    {
        public HyndmanFanType Type { get; }

        public HyndmanFanQuantileEstimator(HyndmanFanType type)
        {
            if (!Enum.IsDefined(typeof(HyndmanFanType), type))
                throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown type");

            Type = type;
        }

        /// <summary>
        /// Returns 1-based real index estimation
        /// </summary>
        private double GetH(double n, Probability p) => HyndmanFanHelper.GetH(Type, n, p);

        public virtual double GetQuantile(Sample sample, Probability probability)
        {
            if (!SupportsWeightedSamples)
                Assertion.NonWeighted(nameof(sample), sample);

            return sample.IsWeighted
                ? GetQuantileForWeightedSample(sample, probability)
                : GetQuantileForNonWeightedSample(sample, probability);
        }

        // See https://aakinshin.net/posts/weighted-quantiles/
        private double GetQuantileForWeightedSample(Sample sample, Probability probability)
        {
            Assertion.NotNull(nameof(sample), sample);

            int n = sample.Count;
            double p = probability;
            double h = GetH(n, p).Clamp(1, n);
            double left = (h - 1) / n;
            double right = h / n;

            double Cdf(double x)
            {
                if (x <= left)
                    return 0;
                if (x >= right)
                    return 1;
                return x * n - h + 1;
            }

            double totalWeight = sample.TotalWeight;
            double result = 0;
            double current = 0;
            for (int i = 0; i < n; i++)
            {
                double next = current + sample.Weights[i] / totalWeight;
                result += sample.SortedValues[i] * (Cdf(next) - Cdf(current));
                current = next;
            }

            return result;
        }

        private double GetQuantileForNonWeightedSample(Sample sample, Probability probability)
        {
            var sortedValues = sample.SortedValues;

            double GetValue(int index)
            {
                index -= 1; // Adapt one-based formula to the zero-based list
                if (index <= 0)
                    return sortedValues[0];
                if (index >= sample.Count)
                    return sortedValues[sample.Count - 1];
                return sortedValues[index];
            }

            return HyndmanFanHelper.Evaluate(Type, sample.Count, probability, GetValue);
        }

        public virtual bool SupportsWeightedSamples => HyndmanFanHelper.SupportsWeightedSamples(Type);
        public string Alias => "HF" + (int)Type;
    }
}