using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
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

        private TukeyOutlierDetector(Quartiles quartiles, double k)
        {
            LowerFence = quartiles.Q1 - k * quartiles.InterquartileRange;
            UpperFence = quartiles.Q3 + k * quartiles.InterquartileRange;
        }

        [NotNull]
        public static TukeyOutlierDetector Create(Quartiles quartiles, double k = DefaultK)
        {
            return new TukeyOutlierDetector(quartiles, k);
        }

        [NotNull]
        public static TukeyOutlierDetector Create(ISortedReadOnlyList<double> values, double k = DefaultK,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
            return new TukeyOutlierDetector(Quartiles.Create(values, quantileEstimator), k);
        }

        [NotNull]
        public static TukeyOutlierDetector Create(IReadOnlyList<double> values, double k = DefaultK,
            [CanBeNull] IQuantileEstimator quantileEstimator = null)
        {
            quantileEstimator ??= HarrellDavisQuantileEstimator.Instance;
            return new TukeyOutlierDetector(Quartiles.Create(values, quantileEstimator), k);
        }
    }
}