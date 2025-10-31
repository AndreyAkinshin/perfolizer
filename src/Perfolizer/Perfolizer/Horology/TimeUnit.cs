using JetBrains.Annotations;
using Perfolizer.Helpers;
using Perfolizer.Mathematics.Common;
using Pragmastat.Metrology;

namespace Perfolizer.Horology;

public class TimeUnit : MeasurementUnit
{
    private readonly string? abbreviationAscii;
    public override string AbbreviationAscii => abbreviationAscii ?? Abbreviation;

    private TimeUnit(string abbreviation, string abbreviationAscii, string fullName, long baseUnits)
        : base(abbreviation, fullName, baseUnits)
    {
        this.abbreviationAscii = abbreviationAscii;
    }

    private TimeUnit(string abbreviation, string fullName, long baseUnits)
        : base(abbreviation, fullName, baseUnits)
    {
    }

    public TimeInterval ToInterval(long value = 1) => new(value, this);

    [PublicAPI] public static readonly TimeUnit Nanosecond = new("ns", "Nanosecond", 1);
    [PublicAPI] public static readonly TimeUnit Microsecond = new($"{UnicodeHelper.Mu}s", "us", "Microsecond", 1000);
    [PublicAPI] public static readonly TimeUnit Millisecond = new("ms", "Millisecond", 1000.PowInt(2));
    [PublicAPI] public static readonly TimeUnit Second = new("s", "Second", 1000.PowInt(3));
    [PublicAPI] public static readonly TimeUnit Minute = new("m", "Minute", Second.BaseUnits * 60);
    [PublicAPI] public static readonly TimeUnit Hour = new("h", "Hour", Minute.BaseUnits * 60);
    [PublicAPI] public static readonly TimeUnit Day = new("d", "Day", Hour.BaseUnits * 24);

    [PublicAPI] public static readonly TimeUnit[] All =
        [Nanosecond, Microsecond, Millisecond, Second, Minute, Hour, Day];

    /// <summary>
    /// This method chooses the best time unit for representing a set of time measurements.
    /// </summary>
    /// <param name="values">The list of time measurements in nanoseconds.</param>
    /// <returns>Best time unit.</returns>
    public static TimeUnit GetBestTimeUnit(params double[] values)
    {
        if (values.Length == 0) return Nanosecond;

        var nonZeroValues = values.Where(x => x > 0).ToList();
        if (nonZeroValues.Count == 0) return Nanosecond;

        // Use the largest unit to display the smallest non-zero recorded measurement without loss of precision.
        double minValue = nonZeroValues.Min();
        return All.LastOrDefault(unit => minValue >= unit.BaseUnits) ?? All.First();
    }

    public static double Convert(double value, TimeUnit from, TimeUnit? to) =>
        value * from.BaseUnits / (to ?? GetBestTimeUnit(value)).BaseUnits;

    public static bool TryParse(string s, out TimeUnit unit)
    {
        foreach (TimeUnit timeUnit in All)
        {
            if (timeUnit.Abbreviation.Equals(s) ||
                timeUnit.AbbreviationAscii.Equals(s) ||
                timeUnit.FullName.Equals(s))
            {
                unit = timeUnit;
                return true;
            }
        }

        unit = Nanosecond;
        return false;
    }

    public static TimeUnit Parse(string s) =>
        TryParse(s, out TimeUnit unit) ? unit : throw new FormatException($"Unknown time unit: {s}");
}