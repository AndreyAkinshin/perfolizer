namespace Perfolizer.Metrology;

public class PercentUnit() : MeasurementUnit("%", "Percent", 1)
{
    public static readonly PercentUnit Instance = new();
}