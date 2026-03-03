using System.Globalization;
using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

public class MeasurementFormatter
{
    public static readonly MeasurementFormatter Default = new();

    public string Format(Measurement measurement, string? format = null, IFormatProvider? formatProvider = null)
    {
        string valueStr = measurement.NominalValue.ToString(format, formatProvider);
        string abbreviation = measurement.Unit.Abbreviation;
        return abbreviation.Length == 0 ? valueStr : valueStr + abbreviation;
    }

    public bool TryParse(string s, out Measurement measurement)
    {
        s = s.Trim();
        int spaceIdx = s.IndexOf(' ');
        string valuePart, unitPart;
        if (spaceIdx >= 0)
        {
            valuePart = s.Substring(0, spaceIdx).Trim();
            unitPart = s.Substring(spaceIdx + 1).Trim();
        }
        else
        {
            int i = 0;
            while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.' || s[i] == '-' || s[i] == '+' ||
                                    s[i] == 'e' || s[i] == 'E'))
                i++;
            valuePart = s.Substring(0, i);
            unitPart = s.Substring(i).Trim();
        }

        if (!double.TryParse(valuePart, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        {
            measurement = default!;
            return false;
        }

        if (string.IsNullOrEmpty(unitPart))
        {
            measurement = new Measurement(value);
            return true;
        }

        foreach (var unit in PerfolizerMeasurementExtensions.GetAll())
        {
            if (unit.Abbreviation.Equals(unitPart, StringComparison.OrdinalIgnoreCase))
            {
                measurement = new Measurement(value, unit);
                return true;
            }
        }

        measurement = default!;
        return false;
    }

    public Measurement Parse(string s)
    {
        if (!TryParse(s, out var measurement))
            throw new FormatException($"Cannot parse measurement: '{s}'");
        return measurement;
    }
}
