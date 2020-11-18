using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Collections;

namespace Perfolizer.Common
{
    public class Sample
    {
        [NotNull] public IReadOnlyList<double> Values { get; }
        [NotNull] public IReadOnlyList<double> Weights { get; }
        public double TotalWeight;
        public bool IsWeighted { get; }

        [NotNull] private readonly Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)> lazySortedData;

        [NotNull] public IReadOnlyList<double> SortedValues => lazySortedData.Value.SortedValues;
        [NotNull] public IReadOnlyList<double> SortedWeights => lazySortedData.Value.SortedWeights;

        public int Count => Values.Count;

        public Sample([NotNull] IReadOnlyList<double> values)
        {
            Assertion.NotNullOrEmpty(nameof(values), values);

            Values = values;
            Weights = new IdenticalReadOnlyList<double>(values.Count, 1.0 / values.Count);
            TotalWeight = 1.0;
            IsWeighted = false;

            lazySortedData = new Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)>(
                () => (Values.CopyToArrayAndSort(), Weights));
        }

        public Sample([NotNull] IReadOnlyList<double> values, [NotNull] IReadOnlyList<double> weights)
        {
            Assertion.NotNullOrEmpty(nameof(values), values);
            Assertion.NotNullOrEmpty(nameof(weights), weights);
            if (values.Count != weights.Count)
                throw new ArgumentException($"{nameof(weights)} should have the same number of elements as {nameof(values)}",
                    nameof(weights));
            if (weights.Any(w => w < 0))
                throw new ArgumentOutOfRangeException(nameof(weights), $"All weights in {nameof(weights)} should be non-negative");
            double totalWeight = weights.Sum();
            if (totalWeight < 1e-9)
                throw new ArgumentException(nameof(weights), $"The sum of all elements from {nameof(weights)} should be positive");

            Values = values;
            Weights = weights;
            TotalWeight = totalWeight;
            IsWeighted = true;

            lazySortedData = new Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)>(() =>
            {
                if (IsSorted(Values))
                    return (Values, Weights);

                double[] sortedValues = Values.CopyToArray();
                double[] sortedWeights = Weights.CopyToArray();
                Array.Sort(sortedValues, sortedWeights);

                return (sortedValues, sortedWeights);
            });
        }

        private static bool IsSorted(IReadOnlyList<double> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
                if (list[i] > list[i + 1])
                    return false;
            return true;
        }
    }
}