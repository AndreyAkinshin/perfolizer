using System;
using System.Collections.Generic;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.Histograms;

namespace Perfolizer.Samples
{
    public class HistogramSample
    {
        public void Run()
        {
            var random = new Random(42);
            var data = new List<double>();
            data.AddRange(new NormalDistribution(mean: 20, stdDev: 1).Random(random).Next(200));
            data.AddRange(new NormalDistribution(mean: 22, stdDev: 1).Random(random).Next(200));

            const double binSize = 0.5;
            Console.WriteLine("*** Simple Histogram ***");
            Console.WriteLine(SimpleHistogramBuilder.Instance.Build(data, binSize).ToString());
            Console.WriteLine("*** Adaptive Histogram ***");
            Console.WriteLine(AdaptiveHistogramBuilder.Instance.Build(data, binSize).ToString());
        }
    }
}