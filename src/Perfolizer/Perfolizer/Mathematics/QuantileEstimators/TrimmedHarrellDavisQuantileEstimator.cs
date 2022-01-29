using System;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using static System.Math;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// Trimmed Harrell-Davis quantile estimator based on the highest density interval of the given width.
    /// <remarks>See https://arxiv.org/abs/2111.11776</remarks>
    /// </summary>
    public class TrimmedHarrellDavisQuantileEstimator : IQuantileEstimator
    {
        private readonly Func<double, Probability> getIntervalWidth;

        public static readonly IQuantileEstimator SqrtInstance = new TrimmedHarrellDavisQuantileEstimator(n => 1.0 / Sqrt(n), "SQRT");

        public TrimmedHarrellDavisQuantileEstimator(Func<double, Probability> getIntervalWidth, string? alias = null)
        {
            this.getIntervalWidth = getIntervalWidth;
            Alias = string.IsNullOrEmpty(alias) ? "THD" : "THD-" + alias;
        }

        public double GetQuantile(Sample sample, Probability probability)
        {
            Assertion.NotNull(nameof(sample), sample);

            double n = sample.WeightedCount;
            double a = (n + 1) * probability, b = (n + 1) * (1 - probability);
            var distribution = new BetaDistribution(a, b);
            var d = getIntervalWidth(n);
            var hdi = GetBetaHdi(a, b, d);
            double hdiCdfL = distribution.Cdf(hdi.L);
            double hdiCdfR = distribution.Cdf(hdi.R);

            double Cdf(double x)
            {
                if (x <= hdi.L)
                    return 0;
                if (x > hdi.R)
                    return 1;
                return (distribution.Cdf(x) - hdiCdfL) / (hdiCdfR - hdiCdfL);
            }

            double c1 = 0;
            double betaCdfRight = 0;
            double currentProbability = 0;
            if (sample.IsWeighted)
            {
                for (int j = 0; j < sample.Count; j++)
                {
                    double betaCdfLeft = betaCdfRight;
                    currentProbability += sample.SortedWeights[j] / sample.TotalWeight;

                    double cdfValue = Cdf(currentProbability);
                    betaCdfRight = cdfValue;
                    double w = betaCdfRight - betaCdfLeft;
                    c1 += w * sample.SortedValues[j];
                }
            }
            else
            {
                int jL = (int)Floor(hdi.L * sample.Count);
                int jR = (int)Ceiling(hdi.R * sample.Count) - 1;
                for (int j = jL; j <= jR; j++)
                {
                    double betaCdfLeft = betaCdfRight;
                    currentProbability = (j + 1.0) / sample.Count;

                    double cdfValue = Cdf(currentProbability);
                    betaCdfRight = cdfValue;
                    double w = betaCdfRight - betaCdfLeft;
                    c1 += w * sample.SortedValues[j];
                }
            }
            return c1;
        }

        public bool SupportsWeightedSamples => true;
        public string Alias { get; }

        private static double BinarySearch(Func<double, double> f, double left, double right)
        {
            double fl = f(left);
            double fr = f(right);
            if (fl < 0 && fr < 0 || fl > 0 && fr > 0)
                return double.NaN;

            while (right - left > 1e-9)
            {
                double m = (left + right) / 2;
                double fm = f(m);
                if (fl < 0 && fm < 0 || fl > 0 && fm > 0)
                {
                    fl = fm;
                    left = m;
                }
                else
                {
                    right = m;
                }
            }

            return (left + right) / 2;
        }

        internal static (double L, double R) GetBetaHdi(double a, double b, double width)
        {
            const double eps = 1e-9;
            if (a < 1 + eps & b < 1 + eps)
                return (0.5 - width / 2, 0.5 + width / 2);
            if (a < 1 + eps && b > 1)
                return (0, width);
            if (a > 1 & b < 1 + eps)
                return (1 - width, 1);
            if (width > 1 - eps)
                return (0, 1);

            double mode = (a - 1) / (a + b - 2);
            var beta = new BetaDistribution(a, b);
            double l = BinarySearch(x => beta.Pdf(x) - beta.Pdf(x + width),
                Max(0, mode - width),
                Min(mode, 1 - width));
            double r = l + width;
            return (l, r);
        }
    }
}