namespace Perfolizer.Metrology;

public static class ApplicableMeasurementUnitExtensions
{
    public static Threshold ToThreshold(this IApplicableMeasurementUnit unit) => new(unit);
}