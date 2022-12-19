using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.Histograms
{
    public class SimpleHistogramBuilder : IHistogramBuilder
    {
        public static readonly IHistogramBuilder Instance = new SimpleHistogramBuilder();
        
        [PublicAPI, Pure]
        public Histogram Build(IReadOnlyList<double> values)
        {
            var moments = Moments.Create(values);
            double binSize = OptimalBinSize(values.Count, moments.StandardDeviation);
            if (Math.Abs(binSize) < 1e-9)
                binSize = 1;
            return Build(values, binSize);
        }

        [PublicAPI, Pure]
        public Histogram Build(IReadOnlyList<double> values, double binSize)
        {
            if (binSize < 1e-9)
                throw new ArgumentException($"binSize ({binSize.ToString("0.##", DefaultCultureInfo.Instance)}) should be a positive number", nameof(binSize));

            if (values.Count == 0)
                throw new ArgumentException("Values should be non-empty", nameof(values));

            var sortedValues = values.CopyToArrayAndSort();

            int firstBin = BinIndex(sortedValues.First(), binSize);
            int lastBin = BinIndex(sortedValues.Last(), binSize);
            int binCount = lastBin - firstBin + 1;

            var bins = new HistogramBin[binCount];
            int counter = 0;
            for (int i = 0; i < bins.Length; i++)
            {
                var bin = new List<double>();
                double lower = (firstBin + i) * binSize;
                double upper = (firstBin + i + 1) * binSize;

                while (counter < sortedValues.Length && (sortedValues[counter] < upper || i == bins.Length - 1))
                    bin.Add(sortedValues[counter++]);

                bins[i] = new HistogramBin(lower, upper, bin.ToArray());
            }

            return new Histogram(binSize, bins);
        }

        private static int BinIndex(double value, double binSize) => (int) Math.Floor(value / binSize);

        [PublicAPI, Pure]
        public static double OptimalBinSize(int n, double standardDeviation)
        {
            return HistogramBinSizeCalculator.CalcScott2(n, standardDeviation);
        }
    }
}