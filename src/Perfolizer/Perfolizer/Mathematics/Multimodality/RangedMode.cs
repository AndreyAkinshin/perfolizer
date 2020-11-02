using System.Collections.Generic;
using Perfolizer.Collections;

namespace Perfolizer.Mathematics.Multimodality
{
    public class RangedMode
    {
        public double Location { get; }
        public double Left { get; }
        public double Right { get; }
        
        public ISortedReadOnlyList<double> Values { get; }

        public RangedMode(double location, double left, double right, ISortedReadOnlyList<double> values)
        {
            Location = location;
            Left = left;
            Right = right;
            Values = values;
        }
    }
}