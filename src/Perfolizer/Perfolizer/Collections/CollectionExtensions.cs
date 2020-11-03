using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Collections
{
    internal static class CollectionExtensions
    {
        [NotNull]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        public static double[] CopyToArray([NotNull] this IEnumerable<double> values)
        {
            switch (values)
            {
                case null:
                    throw new ArgumentOutOfRangeException(nameof(values));
                case double[] array:
                {
                    var result = new double[array.Length];
                    Array.Copy(array, result, array.Length);
                    return result;
                }
                case IReadOnlyList<double> list:
                {
                    var result = new double[list.Count];
                    for (int i = 0; i < list.Count; i++)
                        result[i] = list[i];
                    return result;
                }
                default:
                    return values.ToArray();
            }
        }

        [NotNull]
        public static double[] CopyToArrayAndSort([NotNull] this IEnumerable<double> values)
        {
            var array = values.CopyToArray();
            Array.Sort(array);
            return array;
        }

        [NotNull]
        public static ISortedReadOnlyList<double> ToSorted([NotNull] this IReadOnlyList<double> values)
        {
            if (values is ISortedReadOnlyList<double> sortedValues)
                return sortedValues;
            return SortedReadOnlyDoubleList.Create(values);
        }

        public static double Minimum([NotNull] this ISortedReadOnlyList<double> values) => values.First();
        public static double Maximum([NotNull] this ISortedReadOnlyList<double> values) => values.Last();

        /// <summary>
        /// Returns the index of the minimum element in the given range
        /// </summary>
        internal static int WhichMin([NotNull] this IReadOnlyList<double> source, int start, int length)
        {
            Assertion.NotNullOrEmpty(nameof(source), source);
            Assertion.InRangeInclusive(nameof(start), start, 0, source.Count - 1);
            Assertion.InRangeInclusive(nameof(length), length, 1, source.Count - start);

            double minValue = source[start];
            int minIndex = start;
            for (int i = start + 1; i < start + length; i++)
                if (source[i] < minValue)
                {
                    minValue = source[i];
                    minIndex = i;
                }

            return minIndex;
        }

        /// <summary>
        /// Returns the index of the minimum element
        /// </summary>
        internal static int WhichMin([NotNull] this IReadOnlyList<double> source) => WhichMin(source, 0, source.Count);

        /// <summary>
        /// Returns the index of the maximum element in the given range
        /// </summary>
        internal static int WhichMax([NotNull] this IReadOnlyList<double> source, int start, int length)
        {
            Assertion.NotNullOrEmpty(nameof(source), source);
            Assertion.InRangeInclusive(nameof(start), start, 0, source.Count - 1);
            Assertion.InRangeInclusive(nameof(length), length, 1, source.Count - start);

            double maxValue = source[start];
            int maxIndex = start;
            for (int i = start + 1; i < start + length; i++)
                if (source[i] > maxValue)
                {
                    maxValue = source[i];
                    maxIndex = i;
                }

            return maxIndex;
        }

        /// <summary>
        /// Returns the index of the maximum element
        /// </summary>
        internal static int WhichMax([NotNull] this IReadOnlyList<double> source) => WhichMax(source, 0, source.Count);
    }
}