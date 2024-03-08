namespace Perfolizer.Metrology;

public class PercentageUnit() : MeasurementUnit("%", "Percent", 1)
{
    public static readonly PercentageUnit Instance = new();
}