using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Histograms;
using Perfolizer.Mathematics.OutlierDetection;
using Perfolizer.Mathematics.QuantileEstimators;
using static System.Math;

namespace Perfolizer.Mathematics.Multimodality
{
    public static class MValueCalculator
    {
        public static double Calculate([NotNull] IEnumerable<double> values) => Calculate(values.ToArray());
        
        // See http://www.brendangregg.com/FrequencyTrails/modes.html
        [PublicAPI]
        public static double Calculate([NotNull] double[] values)
        {
            try
            {
                var clearedValues = TukeyOutlierDetector.FromUnsorted(values).WithoutAllOutliers(values);
                int n = clearedValues.Count;
                var quartiles = Quartiles.FromUnsorted(clearedValues);
                var moments = Moments.Create(clearedValues);

                double mValue = 0;

                double binSize = HistogramBinSizeCalculator.CalcScott2(n, moments.StandardDeviation);
                if (Abs(binSize) < 1e-9)
                    binSize = 1;
                while (true)
                {
                    var histogram = HistogramBuilder.Adaptive.Build(clearedValues, binSize);
                    var x = new List<int> { 0 };
                    x.AddRange(histogram.Bins.Select(bin => bin.Count));
                    x.Add(0);

                    int sum = 0;
                    for (int i = 1; i < x.Count; i++)
                        sum += Abs(x[i] - x[i - 1]);
                    mValue = Max(mValue, sum * 1.0 / x.Max());

                    if (binSize > quartiles.Max - quartiles.Min)
                        break;
                    binSize *= 2.0;
                }

                return mValue;
            }
            catch (Exception)
            {
                return 1; // In case of any bugs, we return 1 because it's an invalid value (mValue is always >= 2)
            }
        }
    }
}