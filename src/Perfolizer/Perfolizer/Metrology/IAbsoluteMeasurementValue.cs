namespace Perfolizer.Metrology;

public interface IAbsoluteMeasurementValue : ISpecificMeasurementValue
{
    double GetShift(Sample sample);
}