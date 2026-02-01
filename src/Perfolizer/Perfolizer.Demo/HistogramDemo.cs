using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Histograms;
using Pragmastat.Randomization;

namespace Perfolizer.Demo;

public class HistogramDemo: IDemo
{
    public void Run()
    {
        var rng = new Rng(42);
        var data = new List<double>();
        data.AddRange(new NormalDistribution(mean: 20, stdDev: 1).Random(rng).Next(200));
        data.AddRange(new NormalDistribution(mean: 22, stdDev: 1).Random(rng).Next(200));

        const double binSize = 0.5;
        Console.WriteLine("*** Simple Histogram ***");
        Console.WriteLine(SimpleHistogramBuilder.Instance.Build(data, binSize).ToString());
        Console.WriteLine("*** Adaptive Histogram ***");
        Console.WriteLine(AdaptiveHistogramBuilder.Instance.Build(data, binSize).ToString());
    }
}