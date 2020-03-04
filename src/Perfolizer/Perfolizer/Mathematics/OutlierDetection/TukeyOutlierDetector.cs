using Perfolizer.Mathematics.Quantiles;

namespace Perfolizer.Mathematics.OutlierDetection
{
    public class TukeyOutlierDetector : OutlierDetector
    {
        private readonly Quartiles quartiles;
        private readonly double k;

        public TukeyOutlierDetector(double[] values, double k = 1.5)
        {
            quartiles = Quartiles.FromUnsorted(values);
            this.k = k;
        }

        public override bool IsLowerOutlier(double x) => x < quartiles.Q1 - k * quartiles.InterquartileRange;

        public override bool IsUpperOutlier(double x) => x > quartiles.Q3 + k * quartiles.InterquartileRange;
    }
}