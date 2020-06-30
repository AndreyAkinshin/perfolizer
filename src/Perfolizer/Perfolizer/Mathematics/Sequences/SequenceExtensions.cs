using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Sequences
{
    public static class SequenceExtensions
    {
        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        public static IEnumerable<double> GenerateEnumerable([NotNull] this ISequence sequence)
        {
            int index = 0;
            while (true)
                yield return sequence.GetValue(index++);
        }

        public static double[] GenerateArray([NotNull] this ISequence sequence, int count, bool normalize = false)
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
    }
}