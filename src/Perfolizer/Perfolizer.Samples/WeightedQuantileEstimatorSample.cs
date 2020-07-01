using System;
using System.Linq;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Sequences;

namespace Perfolizer.Samples
{
    public class WeightedQuantileEstimatorSample
    {
        public void Run()
        {
            var values = new NormalDistribution(mean: 20, stdDev: 2).Random(1).Next(20)
                .Concat(new NormalDistribution(mean: 40, stdDev: 2).Random(2).Next(20))
                .Concat(new NormalDistribution(mean: 60, stdDev: 2).Random(3).Next(20))
                .ToArray();

            double GetMedian(int count, int halfLife)
            {
                var sample = values.Take(count).ToList();
                var weights = ExponentialDecaySequence.CreateFromHalfLife(halfLife).GenerateReverseArray(count);
                return HarrellDavisQuantileEstimator.Instance.GetWeightedQuantile(sample, weights, 0.5);
            }

            Console.WriteLine("Size       WeightedMedian");
            Console.WriteLine("       1      3      5     10   // Half-life");
            for (int i = 10; i < values.Length; i++)
            {
                double median1 = GetMedian(i, 1);
                double median3 = GetMedian(i, 3);
                double median5 = GetMedian(i, 5);
                double median10 = GetMedian(i, 10);
                string hint = i % 20 == 0 ? $" // Mean={(i / 20 + 1) * 20}" : "";
                Console.WriteLine($"{i}   {median1:0.00}  {median3:0.00}  {median5:0.00}  {median10:0.00}{hint}");
            }
        }
    }
}