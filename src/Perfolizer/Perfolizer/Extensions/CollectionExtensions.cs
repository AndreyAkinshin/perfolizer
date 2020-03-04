using System;
using System.Collections.Generic;
using System.Linq;

namespace Perfolizer.Extensions
{
    internal static class CollectionExtensions
    {
        public static double[] CopyToArray(this IEnumerable<double> values)
        {
            if (values is double[] array)
            {
                var result = new double[array.Length];
                Array.Copy(array, result, array.Length);
                return result;
            }

            if (values is IReadOnlyList<double> list)
            {
                var result = new double[list.Count];
                for (int i = 0; i < list.Count; i++)
                    result[i] = list[i];
                return result;
            }
            
            return values.ToArray();
        }
    }
}