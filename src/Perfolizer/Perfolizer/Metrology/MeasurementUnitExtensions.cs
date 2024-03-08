using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public static class MeasurementUnitExtensions
{
    public static Threshold ToThreshold(this ISpecificMeasurementValue value) => new (value);

    [PublicAPI]
    public static string ToString(this IWithUnits unit, IFormatProvider? formatProvider, UnitPresentation? unitPresentation = null) =>
        unit.ToString(null, formatProvider, unitPresentation);

    [PublicAPI]
    public static string ToString(this IWithUnits unit, UnitPresentation? unitPresentation) => unit.ToString(null, null, unitPresentation);
}