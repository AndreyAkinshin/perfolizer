namespace Perfolizer.Metrology;

public interface IFormattableUnit
{
    string ToString(string? format, IFormatProvider? formatProvider = null, UnitPresentation? unitPresentation = null);
}