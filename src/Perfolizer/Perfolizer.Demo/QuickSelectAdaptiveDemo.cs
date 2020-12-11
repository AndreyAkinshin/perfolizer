using System;
using Perfolizer.Mathematics.Randomization;
using Perfolizer.Mathematics.Selectors;

namespace Perfolizer.Demo
{
    public class QuickSelectAdaptiveDemo
    {
        public void Run()
        {
            var shuffler = new Shuffler(42);
            var data = new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            shuffler.Shuffle(data);
            
            var selector = new QuickSelectAdaptive();
            for (int i = 0; i < data.Length; i++)
                Console.WriteLine($"data[{i}] = {selector.Select(data, i)}");
        }
    }
}