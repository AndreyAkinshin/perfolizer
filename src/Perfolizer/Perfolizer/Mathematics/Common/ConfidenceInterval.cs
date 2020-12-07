using System;
using System.Text;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common
{
    public readonly struct ConfidenceInterval
    {
        private const string DefaultFormat = "N2";

        public double Estimation { get; }
        public double Lower { get; }
        public double Upper { get; }
        public ConfidenceLevel ConfidenceLevel { get; }

        public ConfidenceInterval(double estimation, double lower, double upper, ConfidenceLevel confidenceLevel)
        {
            Estimation = estimation;
            Lower = lower;
            Upper = upper;
            ConfidenceLevel = confidenceLevel;
        }

        public bool Contains(double value, double eps = 1e-9) => Lower - eps < value && value < Upper + eps;

        public override string ToString() => ToString(DefaultFormat);

        public string ToString(string format, IFormatProvider formatProvider = null, bool showLevel = true)
        {
            formatProvider ??= DefaultCultureInfo.Instance;

            var builder = new StringBuilder();
            builder.Append('[');
            builder.Append(Lower.ToString(format, formatProvider));
            builder.Append("; ");
            builder.Append(Upper.ToString(format, formatProvider));
            builder.Append("]");
            if (showLevel)
            {
                builder.Append(" (CI ");
                builder.Append(ConfidenceLevel.ToString());
                builder.Append(")");
            }

            return builder.ToString();
        }
    }
}