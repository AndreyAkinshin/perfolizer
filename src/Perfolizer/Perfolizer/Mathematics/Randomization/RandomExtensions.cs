using System;

namespace Perfolizer.Mathematics.Randomization
{
    internal static class RandomExtensions
    {
        public static double NextUniform(this Random random)
        {
            return random.NextDouble();
        }
        
        public static double NextUniform(this Random random, double minValue, double maxValue)
        {
            return minValue + (maxValue - minValue) * random.NextDouble();
        }
        
        // See https://stackoverflow.com/questions/218060/random-gaussian-variables
        public static double NextGaussian(this Random random, double mean = 0, double stdDev = 1)
        {
            double stdDevFactor = Math.Sqrt(-2.0 * Math.Log(random.NextDouble())) * Math.Sin(2.0 * Math.PI * random.NextDouble());
            return mean + stdDev * stdDevFactor;
        }
    }
}