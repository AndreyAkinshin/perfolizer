using System;
using System.Linq;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Functions;

namespace Perfolizer.Demo
{
    public class ShiftAndRatioDemo
    {
        public void Run()
        {
            var x = new Sample(
                new NormalDistribution(mean: 20, stdDev: 2).Random(1).Next(20).Concat(
                    new NormalDistribution(mean: 40, stdDev: 2).Random(2).Next(20)).ToArray()
            );
            var y = new Sample(
                new NormalDistribution(mean: 20, stdDev: 2).Random(3).Next(20).Concat(
                    new NormalDistribution(mean: 80, stdDev: 2).Random(4).Next(20)).ToArray()
            );

            var probabilities = new Probability[] {0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0};
            var shift = ShiftFunction.Instance.GetValues(x, y, probabilities);
            var ratio = RatioFunction.Instance.GetValues(x, y, probabilities);

            Console.WriteLine("Probability Shift Ratio");
            for (int i = 0; i < probabilities.Length; i++)
                Console.WriteLine(
                    probabilities[i].ToString("N1").PadRight(11) + " " +
                    shift[i].ToString("N1").PadLeft(5) + " " +
                    ratio[i].ToString("N1").PadLeft(5));

            Console.WriteLine();
            Console.WriteLine("Shift Range: " + ShiftFunction.Instance.GetRange(x, y));
            Console.WriteLine("Ratio Range: " + RatioFunction.Instance.GetRange(x, y));
        }
    }
}