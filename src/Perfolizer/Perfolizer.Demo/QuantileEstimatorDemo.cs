using System;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Demo
{
    public class QuantileEstimatorDemo
    {
        public void Run()
        {
            var x = new double[] {0, 50, 100};
            var probabilities = new Probability[]
                {0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0};
            var simpleQuantiles = SimpleQuantileEstimator.Instance.GetQuantiles(x, probabilities);
            var hdQuantiles = HarrellDavisQuantileEstimator.Instance.GetQuantiles(x, probabilities);

            Console.WriteLine("Probability Simple HarrellDavis");
            for (int i = 0; i < probabilities.Length; i++)
                Console.WriteLine(
                    probabilities[i].ToString("N1").PadRight(11) + " " +
                    simpleQuantiles[i].ToString("N1").PadLeft(6) + " " +
                    hdQuantiles[i].ToString("N1").PadLeft(12));
        }
    }
}