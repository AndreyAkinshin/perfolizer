using System;

namespace Perfolizer.Mathematics.Selectors
{
    public class SimpleSelector
    {
        private readonly double[] data;
        
        public SimpleSelector(double[] data)
        {
            this.data = data;
        }

        public double Select(int k) => Select(0, data.Length - 1, k);

        public double Select(int l, int r, int k)
        {
            var local = new double[r - l + 1];
            for (int i = l; i <= r; i++)
                local[i - l] = data[i];
            Array.Sort(local);
            return local[k];
        }

        public double Median(int l, int r) => Select(l, r, (r - l) / 2);
    }
}