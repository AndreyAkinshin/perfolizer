using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    internal static class QuantileEstimatorHelper
    {
        public static void CheckArguments([CanBeNull] IReadOnlyList<double> data, double quantile)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(data), $"{nameof(data)} should be non-empty");
            if (quantile < 0 || quantile > 1)
                throw new ArgumentOutOfRangeException(nameof(quantile),
                    $"{nameof(quantile)} is {quantile}, but it should be in range [0;1]");
        }
    }
}