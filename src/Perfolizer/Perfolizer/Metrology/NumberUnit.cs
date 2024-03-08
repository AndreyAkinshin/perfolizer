namespace Perfolizer.Metrology;

public class NumberUnit() : MeasurementUnit("", "Number", 1)
{
    public static readonly NumberUnit Instance = new();
}