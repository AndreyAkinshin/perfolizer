namespace Perfolizer.Mathematics.Functions
{
    public static class HypergeometricFunction
    {
        public static double Value(double a, double b, double c, double z, int k)
        {
            double result = 1;
            for (int n = k; n >= 0; n--)
                result = 1 + result * (a + n) * (b + n) / (c + n) * z / (n + 1);
            return result;
        }
    }
}