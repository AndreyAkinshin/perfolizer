using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

public class PerfolizerMeasurementFormatter() : MeasurementFormatter(PerfolizerMeasurementUnitFormatter.Instance)
{
    public static readonly PerfolizerMeasurementFormatter Instance = new();
}