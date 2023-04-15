using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Sequences;

public static class SequenceExtensions
{
    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    public static IEnumerable<double> GenerateEnumerable(this ISequence sequence)
    {
        int index = 0;
        while (true)
            yield return sequence.Value(index++);
    }

    public static double[] GenerateArray(this ISequence sequence, int count, bool normalize = false)
    {
        var values = sequence.GenerateEnumerable().Take(count).ToArray();
        if (normalize)
        {
            double sum = values.Sum();
            for (int i = 0; i < values.Length; i++)
                values[i] /= sum;
        }

        return values;
    }
        
    public static double[] GenerateReverseArray(this ISequence sequence, int count, bool normalize = false)
    {
        var values = sequence.GenerateArray(count, normalize);
        Array.Reverse(values);

        return values;
    }
}