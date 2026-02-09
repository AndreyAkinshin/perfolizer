namespace Perfolizer.Tests.Infra;

public static class Permutator
{
    public static bool NextPermutation(double[] array)
    {
        for (int i = array.Length - 2; i >= 0; i--)
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