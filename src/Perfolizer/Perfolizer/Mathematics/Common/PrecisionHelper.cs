namespace Perfolizer.Mathematics.Common;

public static class PrecisionHelper
{
    public static int GetOptimalPrecision(IReadOnlyList<double> values)
    {
        double minLog = values
            .Where(value => value > 0)
            .Select(Log10)
            .DefaultIfEmpty(0)
            .Min();
        return Max(1, -(minLog - 2).RoundToInt());
    }
}