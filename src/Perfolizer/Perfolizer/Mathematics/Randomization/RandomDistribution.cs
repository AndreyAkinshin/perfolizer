using System;

namespace Perfolizer.Mathematics.Randomization
{
    public class RandomDistribution
    {
        private readonly Random random;

        public RandomDistribution()
        {
            random = new Random();
        }

        public RandomDistribution(int seed)
        {
            random = new Random(seed);
        }

        public RandomDistribution(Random random)
        {
            this.random = random;
        }

        public double[] Uniform(int n, double minValue, double maxValue)
        {
            var values = new double[n];
            for (int i = 0; i < n; i++)
                values[i] = random.NextUniform(minValue, maxValue);
            return values;
        }

        public int[] IntegerUniform(int n, int minValue = 0, int maxValue = 0)
        {
            var values = new int[n];
            for (int i = 0; i < n; i++)
                values[i] = random.Next(minValue, maxValue);
            return values;
        }

        public double[] Gaussian(int n, double mean = 0, double stdDev = 1)
        {
            var values = new double[n];
            for (int i = 0; i < n; i++)
                values[i] = random.NextGaussian(mean, stdDev);
            return values;
        }
    }
}