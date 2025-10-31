using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

public class PerfolizerMeasurementUnitFormatter()
    : MeasurementUnitFormatter(PerfolizerMeasurementExtensions.GetAll().ToList())
{
    public static readonly PerfolizerMeasurementUnitFormatter Instance = new();
}