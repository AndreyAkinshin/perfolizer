using System;
using System.Collections.Generic;

namespace Perfolizer.Examples
{
    static class Program
    {
        private static readonly Dictionary<string, Action> Examples = new Dictionary<string, Action>
        {
            {"ChangePoint", () => new ChangePointExample().Run()},
            {"Histogram", () => new HistogramExample().Run()},
            {"Rqq", () => new RqqExample().Run()},
            {"Multimodal", () => new MultimodalExample().Run()},
            {"QuickSelectAdaptive", () => new QuickSelectAdaptiveExample().Run()},
            {"QuantileEstimator", () => new QuantileEstimatorExample().Run()},
            {"WeightedQuantileEstimator", () => new WeightedQuantileEstimatorExample().Run()},
            {"ShiftAndRatio", () => new ShiftAndRatioExample().Run()},
            {"OutlierDetector", () => new OutlierDetectorExample().Run()},
            {"MultimodalityDetection", () => new MultimodalityDetectionExample().Run()}
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