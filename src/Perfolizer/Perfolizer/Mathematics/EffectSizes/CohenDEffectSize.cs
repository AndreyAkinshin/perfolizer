using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.EffectSizes
{
    public static class CohenDEffectSize
    {
        public static double Calc([NotNull] IReadOnlyList<double> x, [NotNull] IReadOnlyList<double> y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));
            if (x.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} should contain at least 2 elements");
            if (y.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} should contain at least 2 elements");

            int nx = x.Count;
            int ny = y.Count;
            var mx = Moments.Create(x);
            var my = Moments.Create(y);
            double s = Math.Sqrt(((nx - 1) * mx.Variance + (ny - 1) * my.Variance) / (nx + ny - 2));
            return (my.Mean - mx.Mean) / s;
        }
    }
}