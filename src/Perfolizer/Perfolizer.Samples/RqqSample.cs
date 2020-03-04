using System;
using Perfolizer.Mathematics.Selectors;

namespace Perfolizer.Samples
{
    public class RqqSample
    {
        public void Run()
        {
            var data = new double[] {6, 2, 0, 7, 9, 3, 1, 8, 5, 4};
            var rqq = new Rqq(data);
            Console.WriteLine(rqq.DumpTreeAscii());
            Console.WriteLine();
            for (int i = 0; i < data.Length; i++)
                Console.WriteLine($"sorted[{i}] = {rqq.Select(0, data.Length - 1, i)}");
        }
    }
}