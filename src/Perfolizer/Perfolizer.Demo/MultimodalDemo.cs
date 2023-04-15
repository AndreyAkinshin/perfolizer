using Perfolizer.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Multimodality;

namespace Perfolizer.Demo;

public class MultimodalDemo : IDemo
{
    public void Run()
    {
        // In order to detect multimodal distributions, we can use the Lowland multimodality detector
        // * Akinshin, Andrey. "Lowland multimodality detection."
        //   https://aakinshin.net/posts/lowland-multimodality-detection/
        var modalityDetector = LowlandModalityDetector.Instance;
        // We also can use a plain-text notation to describe the details
        // * Akinshin, Andrey. "Plain-text summary notation for multimodal distributions."
        //   https://aakinshin.net/posts/modality-summary-notation/
        var modalityDataFormatter = ManualModalityDataFormatter.Default;

        void DetectModes(string title, Sample sample)
        {
            var modalityData = modalityDetector.DetectModes(sample);
            Console.WriteLine("Distribution : " + title);
            Console.WriteLine("Modality     : " + modalityData.Modality);
            Console.WriteLine("Details      : " + modalityDataFormatter.Format(modalityData));
            Console.WriteLine();
        }

        var random = new Random(42);

        Sample Generate(int n, params IContinuousDistribution[] distributions)
        {
            var values = new List<double>();
            foreach (var distribution in distributions)
                values.AddRange(distribution.Random(random).Next(n));
            return new Sample(values);
        }

        DetectModes("Standard normal distribution", Generate(200,
            NormalDistribution.Standard));
        DetectModes("Standard uniform distribution", Generate(200,
            UniformDistribution.Standard));
        DetectModes("Bimodal distribution", Generate(200,
            new NormalDistribution(10),
            new NormalDistribution(20)));
        DetectModes("Trimodal distribution", Generate(200,
            new NormalDistribution(10),
            new NormalDistribution(20),
            new NormalDistribution(30)));
    }
}