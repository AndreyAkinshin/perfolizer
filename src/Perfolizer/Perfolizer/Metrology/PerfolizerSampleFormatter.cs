using System.Globalization;
using Perfolizer.Horology;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

public class PerfolizerSampleFormatter
{
    public static readonly PerfolizerSampleFormatter Instance = new();

    // Parses "[1,2,3]ms", "[1,2,3]", "1,2,3", etc.
    public Sample Parse(string s)
    {
        s = s.Trim();

        MeasurementUnit unit = MeasurementUnit.Number;
        string valuesPart = s;

        if (s.StartsWith("["))
        {
            int closeBracket = s.LastIndexOf(']');
            if (closeBracket < 0)
                throw new FormatException($"Cannot parse sample: '{s}'");
            valuesPart = s.Substring(1, closeBracket - 1);
            string unitStr = s.Substring(closeBracket + 1).Trim();
            if (!string.IsNullOrEmpty(unitStr))
                unit = ResolveUnit(unitStr);
        }

        var values = valuesPart
            .Split(',')
            .Select(v => double.Parse(v.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture))
            .ToArray();

        return new Sample(values, unit);
    }

    private static MeasurementUnit ResolveUnit(string abbreviation)
    {
        foreach (var u in PerfolizerMeasurementExtensions.GetAll())
        {
            if (u.Abbreviation.Equals(abbreviation, StringComparison.OrdinalIgnoreCase))
                return u;
        }
        throw new FormatException($"Unknown unit: '{abbreviation}'");
    }
}
