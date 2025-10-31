using JetBrains.Annotations;
using Perfolizer.Mathematics.Selectors;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Mathematics.Selectors;

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
        } while (Permutator.NextPermutation(array));

        Assert.Equal(120, permutationsCount);
    }
}