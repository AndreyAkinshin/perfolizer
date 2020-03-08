using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.OutlierDetection
{
    public class TukeyOutlierDetector : OutlierDetector
    {
        private const double DefaultK = 1.5;

        public double LowerFence { get; }
        public double UpperFence { get; }

        private TukeyOutlierDetector(Quartiles quartiles, double k)
        {
            LowerFence = quartiles.Q1 - k * quartiles.InterquartileRange;
            UpperFence = quartiles.Q3 + k * quartiles.InterquartileRange;
        }

        [NotNull]
        public static TukeyOutlierDetector FromQuartiles(Quartiles quartiles, double k = DefaultK)
        {
            return new TukeyOutlierDetector(quartiles, k);
        }

        [NotNull]
        public static TukeyOutlierDetector FromSorted(double[] values, double k = DefaultK)
        {
            return new TukeyOutlierDetector(Quartiles.FromSorted(values), k);
        }

        [NotNull]
        public static TukeyOutlierDetector FromUnsorted(double[] values, double k = DefaultK)
        {
            return new TukeyOutlierDetector(Quartiles.FromUnsorted(values), k);
        }

        public override bool IsLowerOutlier(double x) => x < LowerFence;

        public override bool IsUpperOutlier(double x) => x > UpperFence;
    }
}