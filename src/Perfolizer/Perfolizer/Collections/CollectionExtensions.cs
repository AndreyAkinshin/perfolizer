using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

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
    }
}