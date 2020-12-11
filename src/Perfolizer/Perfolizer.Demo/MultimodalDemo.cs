using System;
using System.Collections.Generic;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.Multimodality;

namespace Perfolizer.Demo
{
    public class MultimodalDemo
    {
        public void Run()
        {
            var random = new Random(42);
            var data = new List<double>();
            data.AddRange(new NormalDistribution(mean: 20, stdDev: 1).Random(random).Next(200));
            data.AddRange(new NormalDistribution(mean: 22, stdDev: 1).Random(random).Next(200));
            List<double>
                unimodal = new List<double>(),
                bimodal = new List<double>(),
                trimodal = new List<double>();
            unimodal.AddRange(new NormalDistribution(mean: 20, stdDev: 1).Random(random).Next(200));
            bimodal.AddRange(new NormalDistribution(mean: 20, stdDev: 1).Random(random).Next(200));
            bimodal.AddRange(new NormalDistribution(mean: 30, stdDev: 1).Random(random).Next(200));
            trimodal.AddRange(new NormalDistribution(mean: 20, stdDev: 1).Random(random).Next(200));
            trimodal.AddRange(new NormalDistribution(mean: 30, stdDev: 1).Random(random).Next(200));
            trimodal.AddRange(new NormalDistribution(mean: 40, stdDev: 1).Random(random).Next(200));
            Console.WriteLine("Unimodal : " + MValueCalculator.Calculate(unimodal));
            Console.WriteLine("Bimodal  : " + MValueCalculator.Calculate(bimodal));
            Console.WriteLine("Trimodal : " + MValueCalculator.Calculate(trimodal));
        }
    }
}