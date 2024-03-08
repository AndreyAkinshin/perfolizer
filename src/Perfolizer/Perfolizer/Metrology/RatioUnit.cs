namespace Perfolizer.Metrology;

public class RatioUnit() : MeasurementUnit("x", "Ratio", 1)
{
    public static readonly RatioUnit Instance = new();
}