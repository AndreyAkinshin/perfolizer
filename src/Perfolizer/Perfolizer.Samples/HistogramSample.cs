using System;
using System.Collections.Generic;
using Perfolizer.Mathematics.Histograms;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Samples
{
    public class HistogramSample
    {
        public void Run()
        {
            var random = new RandomDistribution(42);
            var data = new List<double>();
            data.AddRange(random.Gaussian(200, mean: 20, stdDev: 1));
            data.AddRange(random.Gaussian(200, mean: 22, stdDev: 1));

            const double binSize = 0.5;
            Console.WriteLine("*** Simple Histogram ***");
            Console.WriteLine(SimpleHistogramBuilder.Instance.Build(data, binSize).ToString());
            Console.WriteLine("*** Adaptive Histogram ***");
            Console.WriteLine(AdaptiveHistogramBuilder.Instance.Build(data, binSize).ToString());
        }
    }
}