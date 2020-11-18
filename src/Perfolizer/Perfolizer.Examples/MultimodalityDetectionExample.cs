using System;
using System.Collections.Generic;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.Multimodality;

namespace Perfolizer.Examples
{
    public class MultimodalityDetectionExample
    {
        public void Run()
        {
            var random = new Random(42);
            var data = new List<double>();
            data.AddRange(new[] {1.0, 2.0}); // Lower outliers
            data.AddRange(new NormalDistribution(10, 1).Random(random).Next(100)); // Mode #1
            data.AddRange(new[] {19.0, 21.0}); // Intermodal outliers
            data.AddRange(new NormalDistribution(30, 1).Random(random).Next(100)); // Mode #2
            data.AddRange(new[] {38.0, 39.0}); // Upper outliers

            var modalityData = LowlandModalityDetector.Instance.DetectModes(data);
            Console.WriteLine(AutomaticModalityDataFormatter.Instance.Format(modalityData));
        }
    }
}