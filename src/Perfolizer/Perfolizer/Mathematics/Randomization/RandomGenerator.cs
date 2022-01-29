using System;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Randomization
{
    public abstract class RandomGenerator
    {
        protected readonly Random Random;

        protected RandomGenerator()
        {
            Random = new Random();
        }

        protected RandomGenerator(int seed)
        {
            Random = new Random(seed);
        }

        protected RandomGenerator(Random? random)
        {
            Random = random ?? new Random();
        }

        /// <summary>
        /// Returns a random floating-point number from the given distribution.
        /// </summary>
        /// <returns>A random double-precision floating-point number from the given distribution.</returns>
        public abstract double Next();

        /// <summary>
        /// Returns an array of random floating-point numbers from the given distribution.
        /// </summary>
        /// <param name="n">The size of the returned array.</param>
        /// <returns>An array of random double-precision floating-point numbers from the given distribution.</returns>
        public double[] Next(int n)
        {
            var numbers = new double[n];
            for (int i = 0; i < n; i++)
                numbers[i] = Next();
            return numbers;
        }
    }
}