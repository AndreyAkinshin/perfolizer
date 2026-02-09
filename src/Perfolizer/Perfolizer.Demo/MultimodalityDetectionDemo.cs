using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Multimodality;
using Pragmastat.Randomization;

namespace Perfolizer.Demo;

public class MultimodalityDetectionDemo : IDemo
{
    public void Run()
    {
        var rng = new Rng(42);
        var data = new List<double>();
        data.AddRange(new[] { 1.0, 2.0 }); // Lower outliers
        data.AddRange(new NormalDistribution(10, 1).Random(rng).Next(100)); // Mode #1
        data.AddRange(new[] { 19.0, 21.0 }); // Intermodal outliers
        data.AddRange(new NormalDistribution(30, 1).Random(rng).Next(100)); // Mode #2
        data.AddRange(new[] { 38.0, 39.0 }); // Upper outliers

        var modalityData = LowlandModalityDetector.Instance.DetectModes(data);
        Console.WriteLine(AutomaticModalityDataFormatter.Instance.Format(modalityData));
    }
}