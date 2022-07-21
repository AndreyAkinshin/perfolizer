using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Selectors;
using Xunit;
using Xunit.Sdk;

namespace Perfolizer.Tests.Mathematics.Selectors
{
    [PublicAPI]
    public class QuickSelectAdaptiveAlgorithmsTests
    {
        [Theory]
        [InlineData(new double[]{0, 1, 2, 3, 4})]
        [InlineData(new double[]{1, 2, 3, 4, 5})]
        public void Median5Test(double[] array)
        {
            array.AsSpan().Sort();
            double median = array[2];

            double[] buffer = new double[5];
            int permutationsCount = 0;

            do
            {
                permutationsCount++;
                array.CopyTo(buffer.AsSpan());
                QuickSelectAdaptiveAlgorithms.Median5(buffer, 0, 1, 2, 3, 4);
                Assert.Equal(median, buffer[2]);
            } while (NextPermutation(array));

            Assert.Equal(120, permutationsCount);
        }

        public static bool NextPermutation(double[] array)
        {
            for (int i = array.Length - 2; i >= 0 ; i--)
            {
                if (array[i + 1] <= array[i])
                    continue;

                for (int j = array.Length - 1; j > i; j--)
                {
                    if (array[j] <= array[i])
                        continue;

                    (array[i], array[j]) = (array[j], array[i]);
                    array.AsSpan(i + 1).Reverse();
                    return true;
                }
            }
            return false;
        }
    }
}