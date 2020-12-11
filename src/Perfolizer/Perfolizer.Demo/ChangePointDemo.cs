using System;
using System.Collections.Generic;
using Perfolizer.Mathematics.Cpd;
using Perfolizer.Mathematics.Distributions;

namespace Perfolizer.Demo
{
    public class ChangePointDemo
    {
        public void Run()
        {
            var random = new Random(42);
            var data = new List<double>();
            const int n = 20;
            for (int i = 0; i < n; i++)
                data.AddRange(new NormalDistribution(mean: 20 * i, stdDev: 5).Random(random).Next(100));

            var rqqIndexes = RqqPeltChangePointDetector.Instance.GetChangePointIndexes(data.ToArray());
            var edIndexes = EdPeltChangePointDetector.Instance.GetChangePointIndexes(data.ToArray());

            Console.WriteLine("RqqPelt  EdPelt");
            for (int i = 0, j = 0, k = 1; k < n; k++)
            {
                string rqqIndex = "-", edIndex = "-";
                if (i < rqqIndexes.Length && rqqIndexes[i] < k * 100 + 10)
                    rqqIndex = rqqIndexes[i++].ToString();
                if (j < edIndexes.Length && edIndexes[j] < k * 100 + 10)
                    edIndex = edIndexes[j++].ToString();
                Console.WriteLine($"{rqqIndex.PadLeft(7)} {edIndex.PadLeft(6)}");
            }
            
        }
    }
}