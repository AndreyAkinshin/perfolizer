using System;
using System.Collections.Generic;
using System.Linq;

namespace Perfolizer.Simulations
{
    static class Program
    {
        private static readonly Dictionary<string, Action<string[]>> Simulations = new Dictionary<string, Action<string[]>>
        {
            {"RqqPelt", args => new RqqPeltSimulation().Run(args)},
            {"QuantileCi", args => new QuantileCiSimulation().Run(args)},
        };

        private static void PrintAvailableSimulations()
        {
            Console.WriteLine("Available simulations:");
            foreach (string simulationName in Simulations.Keys)
                Console.WriteLine($"* {simulationName}");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("The first argument should be specified.");
                PrintAvailableSimulations();
                return;
            }

            string simulationName = args[0];
            if (!Simulations.ContainsKey(simulationName))
            {
                Console.WriteLine($"'{simulationName}' is not a valid simulation name.");
                PrintAvailableSimulations();
                return;
            }

            var example = Simulations[simulationName];
            example(args.Skip(1).ToArray());
        }
    }
}