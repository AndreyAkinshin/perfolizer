using System;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// This quantile estimator supports nine popular estimation algorithms that are described in [Hyndman1996].
    /// <remarks>
    /// Hyndman, R. J. and Fan, Y. (1996) Sample quantiles in statistical packages, American Statistician 50, 361–365. doi: 10.2307/2684934.
    /// </remarks>
    /// </summary>
    public class HyndmanFanQuantileEstimator : IQuantileEstimator
    {
        private readonly HyndmanFanType type;

        public HyndmanFanQuantileEstimator(HyndmanFanType type)
        {
            if (!Enum.IsDefined(typeof(HyndmanFanType), type))
                throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown type");

            this.type = type;
        }

        /// <summary>
        /// Returns 1-based real index estimation
        /// </summary>
        protected double GetH(double n, Probability p) => type switch
        {
            HyndmanFanType.Type1 => n * p + 0.5,
            HyndmanFanType.Type2 => n * p + 0.5,
            HyndmanFanType.Type3 => n * p,
            HyndmanFanType.Type4 => n * p,
            HyndmanFanType.Type5 => n * p + 0.5,
            HyndmanFanType.Type6 => (n + 1) * p,
            HyndmanFanType.Type7 => (n - 1) * p + 1,
            HyndmanFanType.Type8 => (n + 1.0 / 3) * p + 1.0 / 3,
            HyndmanFanType.Type9 => (n + 1.0 / 4) * p + 3.0 / 8,
            _ => throw new InvalidOperationException()
        };

        public virtual double GetQuantile(Sample sample, Probability probability)
        {
            Assertion.NonWeighted(nameof(sample), sample);

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

            return HyndmanFanEquations.Evaluate(type, sample.Count, probability, GetValue);
        }

        public virtual bool SupportsWeightedSamples => false;
    }
}