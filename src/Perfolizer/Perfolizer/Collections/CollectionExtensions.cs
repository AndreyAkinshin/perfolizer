using Perfolizer.Common;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Collections;

internal static class CollectionExtensions
{
    public static double[] CopyToArray(this IEnumerable<double> values)
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
                    double[] result = new double[list.Count];
                    for (int i = 0; i < list.Count; i++)
                        result[i] = list[i];
                    return result;
                }
            default:
                return values.ToArray();
        }
    }

    public static double[] CopyToArrayAndSort(this IEnumerable<double> values)
    {
        var array = values.CopyToArray();
        Array.Sort(array);
        return array;
    }

    /// <summary>
    /// Returns the index of the minimum element in the given range
    /// </summary>
    internal static int WhichMin(this IReadOnlyList<double> source, int start, int length)
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
    internal static int WhichMin(this IReadOnlyList<double> source) => WhichMin(source, 0, source.Count);

    /// <summary>
    /// Returns the index of the maximum element in the given range
    /// </summary>
    internal static int WhichMax(this IReadOnlyList<double> source, int start, int length)
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
    internal static int WhichMax(this IReadOnlyList<double> source) => WhichMax(source, 0, source.Count);

    public static Sample ToSample(this IEnumerable<double> values, MeasurementUnit? unit = null)
    {
        Assertion.NotNull(nameof(values), values);
        if (values is IReadOnlyList<double> list)
            return new Sample(list, unit);
        return new Sample(values.ToList(), unit);
    }

    public static bool IsEmpty<T>(this IReadOnlyCollection<T> value) => value.Count == 0;
    public static bool IsNotEmpty<T>(this IReadOnlyCollection<T> value) => value.Count > 0;
    public static bool IsEmpty<T>(this IEnumerable<T> values) => !values.Any();
    public static bool IsNotEmpty<T>(this IEnumerable<T> values) => values.Any();

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> values) =>
        values.Where(value => value != null).Cast<T>();

    public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> values) => values.ToList();

#if NETSTANDARD2_0
    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        => dictionary.TryGetValue(key, out var value) ? value : default;
#endif
}