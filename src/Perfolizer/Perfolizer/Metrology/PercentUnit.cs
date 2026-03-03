using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

public class PercentUnit() : MeasurementUnit("Percent", "Percent", "%", "Percent", 1)
{
    public static readonly PercentUnit Instance = new();
}
