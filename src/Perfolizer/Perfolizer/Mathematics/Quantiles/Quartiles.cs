using System;
using System.Collections.Generic;
using System.Linq;
using Perfolizer.Extensions;

namespace Perfolizer.Mathematics.Quantiles
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

        public Quartiles(double q0, double q1, double q2, double q3, double q4)
        {
            Q0 = q0;
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Q4 = q4;
        }

        public static Quartiles FromSorted(IReadOnlyList<double> values)
        {
            if (values.Count == 0)
                throw new ArgumentException($"{nameof(values)} can't be empty", nameof(values));
            double q0 = values.First();
            double q4 = values.Last();

            double GetMedian(IReadOnlyList<double> x) => x.Count % 2 == 0
                ? (x[x.Count / 2 - 1] + x[x.Count / 2]) / 2
                : x[x.Count / 2];
            
            int n = values.Count;
            double q1, q2, q3;
            if (n == 1)
                q1 = q2 = q3 = values[0];
            else
            {
                // TODO: Optimize
                q1 = GetMedian(values.Take(n / 2).ToList());
                q2 = GetMedian(values);
                q3 = GetMedian(values.Skip((n + 1) / 2).ToList());
            }

            return new Quartiles(q0, q1, q2, q3, q4);
        }

        public static Quartiles FromUnsorted(IReadOnlyList<double> values)
        {
            var sortedValues = values.CopyToArray();
            Array.Sort(sortedValues);
            return FromSorted(sortedValues);
        }
    }
}