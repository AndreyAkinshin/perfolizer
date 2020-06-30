using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Collections;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    internal static class QuantileEstimatorHelper
    {
        [AssertionMethod]
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

        [AssertionMethod]
        private static void CheckWeights([NotNull] IReadOnlyList<double> weights)
        {
            if (weights.Sum() < 1e-9)
                throw new ArgumentException(nameof(weights), $"The sum of all elements from {nameof(weights)} should be positive");
            if (weights.Any(w => w < 0))
                throw new ArgumentOutOfRangeException(nameof(weights), $"All elements in {nameof(weights)} should be non-negative");
        }

        [AssertionMethod]
        public static void CheckWeightedArguments([CanBeNull] IReadOnlyList<double> data, [CanBeNull] IReadOnlyList<double> weights,
            double probability)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (weights == null)
                throw new ArgumentNullException(nameof(weights));

            CheckArguments(data, probability);

            if (weights.Count != data.Count)
                throw new ArgumentException($"{nameof(weights)} should have the same number of elements as {nameof(data)}",
                    nameof(weights));

            CheckWeights(weights);
        }
        
        public static (ISortedReadOnlyList<double> sortedData, IReadOnlyList<double> sortedWeights) SortWeightedData(
            [NotNull] IReadOnlyList<double> data, [NotNull] IReadOnlyList<double> weights)
        {
            var items = new (double value, double weight)[data.Count];
            for (int i = 0; i < data.Count; i++)
                items[i] = (data[i], weights[i]);
            Array.Sort(items, (item1, item2) => item1.value.CompareTo(item2.value));

            var sortedData = items.Select(item => item.value).ToList().ToSorted();
            var sortedWeights = items.Select(item => item.weight).ToList();

            return (sortedData, sortedWeights);
        }
    }
}