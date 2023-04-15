namespace Perfolizer.Mathematics.Multimodality;

public interface IModalityDataFormatter
{
    string Format(ModalityData data, string? numberFormat = null, IFormatProvider? numberFormatProvider = null);
}