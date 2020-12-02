using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.RangeEstimators;

namespace Perfolizer.Mathematics.EffectSizes
{
    public static class GammaEffectSize
    {
        private const double Epsilon = 1e-9;

        public static Range CalcRange([NotNull] Sample x, [NotNull] Sample y, IReadOnlyList<Probability> probabilities)
        {
            Assertion.NotNull(nameof(x), x);
            Assertion.NotNull(nameof(y), y);
            Assertion.NotNullOrEmpty(nameof(probabilities), probabilities);

            try
            {
                int k = probabilities.Count;
                var quantileEstimator = HarrellDavisQuantileEstimator.Instance;
                double xMad = MedianAbsoluteDeviation.CalcMad(x);
                double yMad = MedianAbsoluteDeviation.CalcMad(y);
                if (xMad < Epsilon && yMad < Epsilon)
                {
                    double mx = quantileEstimator.GetQuantile(x, 0.5);
                    double my = quantileEstimator.GetQuantile(y, 0.5);
                    if (Math.Abs(mx - my) < Epsilon)
                        return Range.Zero;
                    return mx < my ? Range.PositiveInfinity : Range.NegativeInfinity;
                }

                double[] qx = quantileEstimator.GetQuantiles(x, probabilities);
                double[] qy = quantileEstimator.GetQuantiles(y, probabilities);

                int nx = x.Count;
                int ny = y.Count;
                double xyMad = Math.Sqrt(((nx - 1) * xMad * xMad + (ny - 1) * yMad * yMad) / (nx + ny - 2));

                var gammas = Enumerable.Range(0, k).Select(i => (qy[i] - qx[i]) / xyMad).ToList();
                double min = gammas.Min();
                double max = gammas.Max();

                return Range.Of(min, max);
            }
            catch (Exception)
            {
                return Range.NaN;
            }
        }

        public static Range CalcRange([NotNull] Sample x, [NotNull] Sample y, double p = 0.2, int? count = null)
        {
            Assertion.NotNull(nameof(x), x);
            Assertion.NotNull(nameof(y), y);
            Assertion.InRangeInclusive(nameof(p), p, 0, 0.5);
            if (count.HasValue)
                Assertion.Positive(nameof(count), count.Value);

            int k = count ?? Math.Max(1, (int) Math.Floor((1 - 2 * p) / 0.01));
            var probabilities = k == 1
                ? new List<Probability> {0.5}
                : Enumerable.Range(0, k).Select(i => (Probability)(p + (1 - 2 * p) * i / (k - 1))).ToList();

            return CalcRange(x, y, probabilities);
        }

        public static double CalcValue([NotNull] Sample x, [NotNull] Sample y, Probability p)
        {
            return CalcRange(x, y, new[] {p}).Middle;
        }
    }
}