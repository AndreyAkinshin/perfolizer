using System;
using System.Collections.Generic;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Mathematics.Randomization;

namespace Perfolizer.Samples
{
    public class MultimodalSample
    {
        public void Run()
        {
            var random = new RandomDistribution(42);
            var data = new List<double>();
            data.AddRange(random.Gaussian(200, mean: 20));
            data.AddRange(random.Gaussian(200, mean: 22));
            List<double>
                unimodal = new List<double>(),
                bimodal = new List<double>(),
                trimodal = new List<double>();
            unimodal.AddRange(random.Gaussian(200, mean: 20));
            bimodal.AddRange(random.Gaussian(200, mean: 20));
            bimodal.AddRange(random.Gaussian(200, mean: 30));
            trimodal.AddRange(random.Gaussian(200, mean: 20));
            trimodal.AddRange(random.Gaussian(200, mean: 30));
            trimodal.AddRange(random.Gaussian(200, mean: 40));
            Console.WriteLine("Unimodal : " + MValueCalculator.Calculate(unimodal));
            Console.WriteLine("Bimodal  : " + MValueCalculator.Calculate(bimodal));
            Console.WriteLine("Trimodal : " + MValueCalculator.Calculate(trimodal));
        }
    }
}