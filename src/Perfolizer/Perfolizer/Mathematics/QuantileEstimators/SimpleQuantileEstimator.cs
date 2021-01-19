using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// The most common quantile estimator, also known as Type 7 (see [Hyndman 1996]).
    /// Consistent with many other statistical packages like R, Julia, NumPy, Excel (`PERCENTILE`, `PERCENTILE.INC`), Python (`inclusive`).
    /// <remarks>
    /// Hyndman, R. J. and Fan, Y. (1996) Sample quantiles in statistical packages, American Statistician 50, 361–365. doi: 10.2307/2684934.
    /// </remarks>
    /// </summary>
    public class SimpleQuantileEstimator : HyndmanFanQuantileEstimator
    {
        public static readonly IQuantileEstimator Instance = new SimpleQuantileEstimator();

        private SimpleQuantileEstimator() : base(HyndmanFanType.Type7)
        {
        }
        
        public override double GetQuantile(Sample sample, Probability probability)
        {
            if (!sample.IsWeighted)
                return base.GetQuantile(sample, probability);
            
            Assertion.NotNull(nameof(sample), sample);

            int n = sample.Count;
            double p = probability;
            double h = GetH(n, p);
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

        public override bool SupportsWeightedSamples => true;
    }
}