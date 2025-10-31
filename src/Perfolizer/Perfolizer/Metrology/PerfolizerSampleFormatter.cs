using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

public class PerfolizerSampleFormatter() : SampleFormatter(PerfolizerMeasurementUnitFormatter.Instance)
{
    public static readonly PerfolizerSampleFormatter Instance = new();
}