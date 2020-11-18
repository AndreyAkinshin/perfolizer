using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Multimodality
{
    public class RangedMode
    {
        public double Location { get; }
        public double Left { get; }
        public double Right { get; }

        [NotNull] public Sample Sample { get; }

        [NotNull] public IReadOnlyList<double> Values => Sample.SortedValues;

        public double Min() => Sample.SortedValues.First();
        public double Max() => Sample.SortedValues.Last();

        public RangedMode(double location, double left, double right, [NotNull] Sample sample)
        {
            Assertion.NonWeighted(nameof(sample), sample);
            Location = location;
            Left = left;
            Right = right;
            Sample = sample;
        }
    }
}