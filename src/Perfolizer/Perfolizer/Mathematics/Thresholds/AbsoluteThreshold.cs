using System;
using System.Collections.Generic;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Thresholds
{
    public class AbsoluteThreshold : Threshold
    {
        private readonly double value;

        public AbsoluteThreshold(double value)
        {
            this.value = value;
        }

        public override double GetValue(IReadOnlyList<double> values) => value;

        public override bool IsZero() => Math.Abs(value) < 1e-9;
        public override string ToString() => value.ToString("0.##", DefaultCultureInfo.Instance);
    }
}