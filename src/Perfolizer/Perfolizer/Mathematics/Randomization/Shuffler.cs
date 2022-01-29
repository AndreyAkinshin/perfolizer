using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Randomization
{
    public class Shuffler
    {
        private readonly Random random;

        public Shuffler()
        {
            random = new Random();
        }

        public Shuffler(int seed)
        {
            random = new Random(seed);
        }

        public Shuffler(Random random)
        {
            this.random = random;
        }

        public void Shuffle<T>(IList<T> data)
        {
            Shuffle(data, 0, data.Count);
        }

        public void Shuffle<T>(IList<T> data, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int j = random.Next(count);
                var temp = data[offset + i];
                data[offset + i] = data[offset + j];
                data[offset + j] = temp;
            }
        }
    }
}