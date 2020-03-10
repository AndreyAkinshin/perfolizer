using System;
using System.Collections.Generic;

namespace Perfolizer.Samples
{
    static class Program
    {
        private static readonly Dictionary<string, Action> Samples = new Dictionary<string, Action>
        {
            {"ChangePoint", () => new ChangePointSample().Run()},
            {"Histogram", () => new HistogramSample().Run()},
            {"Rqq", () => new RqqSample().Run()},
            {"Multimodal", () => new MultimodalSample().Run()},
            {"QuickSelectAdaptive", () => new QuickSelectAdaptiveSample().Run()},
            {"QuantileEstimator", () => new QuantileEstimatorSample().Run()},
            {"ShiftAndRatio", () => new ShiftAndRatioSample().Run()}
        };

        private static void PrintAvailableSamples()
        {
            Console.WriteLine("Available samples:");
            foreach (string sampleName in Samples.Keys)
                Console.WriteLine($"* {sampleName}");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("The first argument should be a sample name.");
                PrintAvailableSamples();
                return;
            }

            string sampleName = args[0];
            if (!Samples.ContainsKey(sampleName))
            {
                Console.WriteLine($"'{sampleName}' is not a valid sample name.");
                PrintAvailableSamples();
                return;
            }

            var sample = Samples[sampleName];
            sample();
        }
    }
}