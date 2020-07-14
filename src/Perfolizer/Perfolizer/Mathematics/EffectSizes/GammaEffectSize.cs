using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.RangeEstimators;

namespace Perfolizer.Mathematics.EffectSizes
{
    public static class GammaEffectSize
    {
        public static Range CalcRange([NotNull] IReadOnlyList<double> x, [NotNull] IReadOnlyList<double> y,
            IReadOnlyList<double> probabilities)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));
            if (x.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} should contain at least 1 element");
            if (y.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} should contain at least 1 element");
            for (int i = 0; i < probabilities.Count; i++)
            {
                double p = probabilities[i];
                if (p < 0 || p > 1)
                    throw new ArgumentOutOfRangeException(nameof(probabilities), $"{nameof(probabilities)}[{i}] should belong [0; 1]");
            }

            int k = probabilities.Count;
            var quantileEstimator = HarrellDavisQuantileEstimator.Instance;
            var qx = quantileEstimator.GetQuantiles(x, probabilities);
            var qy = quantileEstimator.GetQuantiles(y, probabilities);

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

        public static Range CalcRange([NotNull] IReadOnlyList<double> x, [NotNull] IReadOnlyList<double> y, double p = 0.2,
            int? count = null)
        {
            if (p < 0 || p > 0.5)
                throw new ArgumentOutOfRangeException(nameof(p), $"{nameof(p)} should belong [0; 0.5]");
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} should be positive");

            int k = count ?? Math.Max(1, (int) Math.Floor((1 - 2 * p) / 0.01));
            var probabilities = k == 1
                ? new List<double> {0.5}
                : Enumerable.Range(0, k).Select(i => p + (1 - 2 * p) * i / (k - 1)).ToList();

            return CalcRange(x, y, probabilities);
        }

        public static double CalcValue([NotNull] IReadOnlyList<double> x, [NotNull] IReadOnlyList<double> y, double p = 0.5)
        {
            return CalcRange(x, y, new[] {p}).Middle;
        }
    }
}