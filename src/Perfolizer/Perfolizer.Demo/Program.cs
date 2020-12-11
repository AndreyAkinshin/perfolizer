using System;
using System.Collections.Generic;

namespace Perfolizer.Demo
{
    static class Program
    {
        private static readonly Dictionary<string, Action> Examples = new Dictionary<string, Action>
        {
            {"ChangePoint", () => new ChangePointDemo().Run()},
            {"Histogram", () => new HistogramDemo().Run()},
            {"Rqq", () => new RqqDemo().Run()},
            {"Multimodal", () => new MultimodalDemo().Run()},
            {"QuickSelectAdaptive", () => new QuickSelectAdaptiveDemo().Run()},
            {"QuantileEstimator", () => new QuantileEstimatorDemo().Run()},
            {"WeightedQuantileEstimator", () => new WeightedQuantileEstimatorDemo().Run()},
            {"ShiftAndRatio", () => new ShiftAndRatioDemo().Run()},
            {"OutlierDetector", () => new OutlierDetectorDemo().Run()},
            {"MultimodalityDetection", () => new MultimodalityDetectionDemo().Run()}
        };

        private static void PrintAvailableExamples()
        {
            Console.WriteLine("Available examples:");
            foreach (string exampleName in Examples.Keys)
                Console.WriteLine($"* {exampleName}");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("The first argument should be specified.");
                PrintAvailableExamples();
                return;
            }

            string exampleName = args[0];
            if (!Examples.ContainsKey(exampleName))
            {
                Console.WriteLine($"'{exampleName}' is not a valid example name.");
                PrintAvailableExamples();
                return;
            }

            var example = Examples[exampleName];
            example();
        }
    }
}