using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    internal static class QuantileEstimatorHelper
    {
        public static void CheckArguments([CanBeNull] IReadOnlyList<double> data, double probability)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(data), $"{nameof(data)} should be non-empty");
            if (probability < 0 || probability > 1)
                throw new ArgumentOutOfRangeException(nameof(probability),
                    $"{nameof(probability)} is {probability}, but it should be in range [0;1]");
        }
    }
}