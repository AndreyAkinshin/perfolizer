using System;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common
{
    public readonly struct Probability
    {
        public static readonly Probability Zero = 0.0;
        public static readonly Probability Half = 0.5;
        public static readonly Probability One = 1.0;

        public readonly double Value;

        public Probability(double value)
        {
            Assertion.InRangeInclusive(nameof(value), value, 0, 1);
            Value = value;
        }

        public static implicit operator double(Probability probability) => probability.Value;
        public static implicit operator Probability(double value) => new Probability(value);

        public override string ToString()
        {
            return Value.ToString(DefaultCultureInfo.Instance);
        }

        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            return Value.ToString(format, formatProvider ?? DefaultCultureInfo.Instance);
        }

        public static Probability[] ToProbabilities([CanBeNull] double[] values)
        {
            if (values == null)
                return null;
            var probabilities = new Probability[values.Length];
            for (int i = 0; i < values.Length; i++)
                probabilities[i] = values[i];
            return probabilities;
        }
    }
}