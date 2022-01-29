using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Common
{
    public class Sample
    {
        public IReadOnlyList<double> Values { get; }
        public IReadOnlyList<double> Weights { get; }
        public double TotalWeight { get; }
        public bool IsWeighted { get; }

        private readonly Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)> lazySortedData;

        public IReadOnlyList<double> SortedValues => lazySortedData.Value.SortedValues;
        public IReadOnlyList<double> SortedWeights => lazySortedData.Value.SortedWeights;

        /// <summary>
        /// Sample size
        /// </summary>
        public int Count => Values.Count;

        /// <summary>
        /// Kish's Effective Sample Size
        /// </summary>
        public double WeightedCount { get; }

        public Sample(IReadOnlyList<double> values)
        {
            Assertion.NotNullOrEmpty(nameof(values), values);

            Values = values;
            double weight = 1.0 / values.Count;
            Weights = new IdenticalReadOnlyList<double>(values.Count, weight);
            TotalWeight = 1.0;
            WeightedCount = values.Count;
            IsWeighted = false;

            lazySortedData = new Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)>(() =>
            {
                if (IsSorted(Values))
                    return (Values, Weights);
                return (Values.CopyToArrayAndSort(), Weights);
            });
        }

        public Sample(IReadOnlyList<double> values, IReadOnlyList<double> weights)
        {
            Assertion.NotNullOrEmpty(nameof(values), values);
            Assertion.NotNullOrEmpty(nameof(weights), weights);
            if (values.Count != weights.Count)
                throw new ArgumentException($"{nameof(weights)} should have the same number of elements as {nameof(values)}",
                    nameof(weights));

            double totalWeight = 0, maxWeight = double.MinValue, minWeight = double.MaxValue;
            double totalWeightSquared = 0; // Sum of weight squares
            foreach (double weight in weights)
            {
                totalWeight += weight;
                totalWeightSquared += weight.Sqr();
                maxWeight = Math.Max(maxWeight, weight);
                minWeight = Math.Min(minWeight, weight);
            }
            
            if (minWeight < 0)
                throw new ArgumentOutOfRangeException(nameof(weights), $"All weights in {nameof(weights)} should be non-negative");
            if (totalWeight < 1e-9)
                throw new ArgumentException(nameof(weights), $"The sum of all elements from {nameof(weights)} should be positive");

            Values = values;
            Weights = weights;
            TotalWeight = totalWeight;
            WeightedCount = totalWeight.Sqr() / totalWeightSquared;
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