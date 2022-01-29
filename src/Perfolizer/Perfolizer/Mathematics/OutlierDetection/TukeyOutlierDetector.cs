using System;
using System.Collections.Generic;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.OutlierDetection
{
    /// <summary>
    /// Outlier detector based on Tukey's fences.
    /// Consider all values outside [Q1 - k * IQR, Q3 + k * IQR] as outliers.
    /// <remarks>
    /// By default, it uses the Harrell Davis quantile estimator and k = 1.5.
    /// </remarks>
    /// </summary>
    public class TukeyOutlierDetector : FenceOutlierDetector
    {
        private const double DefaultK = 1.5;

        private static readonly TukeyOutlierDetector EmptySampleDetector = new TukeyOutlierDetector(double.MinValue, double.MaxValue);

        private TukeyOutlierDetector(double lowerFence, double upperFence)
        {
            LowerFence = lowerFence;
            UpperFence = upperFence;
        }

        private TukeyOutlierDetector(Quartiles quartiles, double k)
        {
            LowerFence = quartiles.Q1 - k * quartiles.InterquartileRange;
            UpperFence = quartiles.Q3 + k * quartiles.InterquartileRange;
        }

        public static TukeyOutlierDetector Create(Quartiles quartiles, double k = DefaultK)
        {
            return new TukeyOutlierDetector(quartiles, k);
        }

        public static TukeyOutlierDetector Create(Sample sample, double k = DefaultK,
            IQuantileEstimator? quantileEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);

            quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
            return new TukeyOutlierDetector(Quartiles.Create(sample, quantileEstimator), k);
        }

        public static TukeyOutlierDetector Create(IReadOnlyList<double> values, double k = DefaultK,
            IQuantileEstimator? quantileEstimator = null)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Count == 0)
                return EmptySampleDetector;

            quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
            return new TukeyOutlierDetector(Quartiles.Create(values, quantileEstimator), k);
        }
    }
}