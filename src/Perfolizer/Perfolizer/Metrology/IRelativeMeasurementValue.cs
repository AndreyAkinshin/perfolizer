namespace Perfolizer.Metrology;

public interface IRelativeMeasurementValue : ISpecificMeasurementValue
{
    double GetRatio();
}