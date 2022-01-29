using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public readonly struct Quartiles
    {
        public double Q0 { get; }
        public double Q1 { get; }
        public double Q2 { get; }
        public double Q3 { get; }
        public double Q4 { get; }

        public double Min => Q0;
        public double Median => Q2;
        public double Max => Q4;
        public double InterquartileRange => Q3 - Q1;

        private Quartiles(double q0, double q1, double q2, double q3, double q4)
        {
            Q0 = q0;
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Q4 = q4;
        }

        public static Quartiles Create(Sample sample,
            IQuantileEstimator? quantileEstimator = null)
        {
            Assertion.NotNull(nameof(sample), sample);
            quantileEstimator ??= SimpleQuantileEstimator.Instance;

            double GetQuantile(double q) => quantileEstimator.GetQuantile(sample, q);

            double q0 = GetQuantile(0.00);
            double q1 = GetQuantile(0.25);
            double q2 = GetQuantile(0.50);
            double q3 = GetQuantile(0.75);
            double q4 = GetQuantile(1.00);

            return new Quartiles(q0, q1, q2, q3, q4);
        }

        public static Quartiles Create(
            IReadOnlyList<double> values,
            IQuantileEstimator? quantileEstimator = null)
        {
            Assertion.NotNull(nameof(values), values);

            return Create(new Sample(values), quantileEstimator);
        }
    }
}