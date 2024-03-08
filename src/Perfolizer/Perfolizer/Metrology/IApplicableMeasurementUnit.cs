using Perfolizer.Common;

namespace Perfolizer.Metrology;

public interface IApplicableMeasurementUnit : IFormattableUnit
{
    Sample? Apply(Sample sample);
}