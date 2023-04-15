using System;
using System.Linq;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Selectors;

/// <summary>
/// <remarks>
/// Based on http://erdani.com/research/sea2017.pdf
/// </remarks>
/// </summary>
public class QuickSelectAdaptive
{
    private double[]? buffer;

    /// <summary>
    /// Returns the k-th smallest element in the given array.
    /// </summary>
    public double Select(double[] values, int k)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        if (values.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(values), "values.Length should be positive");
        if (k < 0)
            throw new ArgumentOutOfRangeException(nameof(k), $"k ({k}) should be positive");
        if (k >= values.Length)
            throw new ArgumentOutOfRangeException(nameof(k),
                $"k ({k}) should be less than values.Length ({values.Length})");

        if (k == 0)
            return values.Min();
        if (k == values.Length - 1)
            return values.Max();

        return Select(values, k, 0, values.Length - 1);
    }
        
    /// <summary>
    /// Returns the k-th smallest element in the given array.
    /// </summary>
    public double Select(double[] values, int k, int l, int r)
    {
        // TODO: more checks
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        if (values.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(values), "values.Length should be positive");
        if (k < 0)
            throw new ArgumentOutOfRangeException(nameof(k), $"k ({k}) should be positive");
        if (k >= r - l + 1)
            throw new ArgumentOutOfRangeException(nameof(k),
                $"k ({k}) should be less than the number of elements ({r - l + 1})");

        if (buffer == null || buffer.Length < r - l + 1)
            buffer = new double[r - l + 1];
        Array.Copy(values, l, buffer, 0, r - l + 1);

        QuickSelectAdaptiveAlgorithms.QuickSelectAdaptive(new Span<double>(buffer, 0, r - l + 1), k);
        return buffer[k];
    }
}