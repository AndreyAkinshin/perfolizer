using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.RangeEstimators;

namespace Perfolizer.Mathematics.EffectSizes
{
    public static class GammaEffectSize
    {
        public static Range CalcRange([NotNull] Sample x, [NotNull] Sample y, IReadOnlyList<double> probabilities)
        {
            Assertion.NotNull(nameof(x), x);
            Assertion.NotNull(nameof(y), y);
            Assertion.InRangeInclusive(nameof(probabilities), probabilities, 0, 1);

            int k = probabilities.Count;
            var quantileEstimator = HarrellDavisQuantileEstimator.Instance;
            double[] qx = quantileEstimator.GetQuantiles(x, probabilities);
            double[] qy = quantileEstimator.GetQuantiles(y, probabilities);

            double xMad = MedianAbsoluteDeviation.CalcMad(x);
            double yMad = MedianAbsoluteDeviation.CalcMad(y);
            int nx = x.Count;
            int ny = y.Count;
            double xyMad = Math.Sqrt(((nx - 1) * xMad * xMad + (ny - 1) * yMad * yMad) / (nx + ny - 2));

            var gammas = Enumerable.Range(0, k).Select(i => (qy[i] - qx[i]) / xyMad).ToList();
            double min = gammas.Min();
            double max = gammas.Max();

            return Range.Of(min, max);
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
                ? new List<double> {0.5}
                : Enumerable.Range(0, k).Select(i => p + (1 - 2 * p) * i / (k - 1)).ToList();

            return CalcRange(x, y, probabilities);
        }

        public static double CalcValue([NotNull] Sample x, [NotNull] Sample y, double p = 0.5)
        {
            return CalcRange(x, y, new[] {p}).Middle;
        }
    }
}