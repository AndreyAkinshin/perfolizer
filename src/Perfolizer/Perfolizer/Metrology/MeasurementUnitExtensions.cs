using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public static class MeasurementUnitExtensions
{
    public static Threshold ToThreshold(this IApplicableMeasurementUnit unit) => new (unit);

    [PublicAPI]
    public static string ToString(this IFormattableUnit unit, IFormatProvider? formatProvider, UnitPresentation? unitPresentation = null) =>
        unit.ToString(null, formatProvider, unitPresentation);

    [PublicAPI]
    public static string ToString(this IFormattableUnit unit, UnitPresentation? unitPresentation) => unit.ToString(null, null, unitPresentation);
}