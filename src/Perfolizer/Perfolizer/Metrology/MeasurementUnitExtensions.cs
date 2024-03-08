using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public static class MeasurementUnitExtensions
{
    public static Threshold ToThreshold(this IApplicableMeasurementUnit unit) => new(unit);

    [PublicAPI]
    public static string ToString(
        this IFormattableUnit unit,
        IFormatProvider? formatProvider,
        UnitPresentation? unitPresentation = null)
    {
        return unit.ToString(null, formatProvider, unitPresentation);
    }
}