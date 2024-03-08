namespace Perfolizer.Metrology;

public interface IWithUnits
{
    MeasurementUnit Unit { get; }
    string ToString(string? format, IFormatProvider? formatProvider = null, UnitPresentation? unitPresentation = null);
}