using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Metrology;

/// <summary>
/// Size units. Assuming that 1 KB = 1024 B, 1 MB = 1024 KB, 1 GB = 1024 MB, 1 TB = 1024 GB.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")] // We want to use "KB", "MB", "GB", "TB"
public class SizeUnit(string abbreviation, string fullName, long baseUnits)
    : MeasurementUnit(abbreviation, fullName, baseUnits)
{
    private const long BytesInKiloByte = 1024L; // this value MUST NOT be changed

    public SizeValue ToValue(long value = 1) => new(value, this);

    [PublicAPI] public static readonly SizeUnit B = new("B", "Byte", 1L);
    [PublicAPI] public static readonly SizeUnit KB = new("KB", "Kilobyte", BytesInKiloByte);
    [PublicAPI] public static readonly SizeUnit MB = new("MB", "Megabyte", BytesInKiloByte.Pow(2));
    [PublicAPI] public static readonly SizeUnit GB = new("GB", "Gigabyte", BytesInKiloByte.Pow(3));
    [PublicAPI] public static readonly SizeUnit TB = new("TB", "Terabyte", BytesInKiloByte.Pow(4));
    [PublicAPI] public static readonly SizeUnit[] All = [B, KB, MB, GB, TB];

    public static SizeUnit GetBestSizeUnit(params long[] values)
    {
        if (!values.Any())
            return B;

        // Use the largest unit to display the smallest recorded measurement without loss of precision.
        long minValue = values.Min();
        foreach (var sizeUnit in All)
        {
            if (minValue < sizeUnit.BaseUnits * BytesInKiloByte)
                return sizeUnit;
        }
        return All.Last();
    }

    public static double Convert(long value, SizeUnit from, SizeUnit to) =>
        value * (double)from.BaseUnits / to.BaseUnits;
}