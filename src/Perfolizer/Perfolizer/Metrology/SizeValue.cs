using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;

namespace Perfolizer.Metrology;

[SuppressMessage("ReSharper", "InconsistentNaming")] // We want to use "KB", "MB", "GB", "TB"
public readonly struct SizeValue(long bytes)
{
    private const string DefaultFormat = "0.##";

    public long Bytes { get; } = bytes;

    public SizeValue(long nominalValue, SizeUnit unit) : this(nominalValue * unit.BaseUnits)
    {
    }

    [PublicAPI] public static readonly SizeValue B = SizeUnit.B.ToValue();
    [PublicAPI] public static readonly SizeValue KB = SizeUnit.KB.ToValue();
    [PublicAPI] public static readonly SizeValue MB = SizeUnit.MB.ToValue();
    [PublicAPI] public static readonly SizeValue GB = SizeUnit.GB.ToValue();
    [PublicAPI] public static readonly SizeValue TB = SizeUnit.TB.ToValue();

    [Pure, PublicAPI] public static SizeValue FromBytes(long value) => value * B;
    [Pure, PublicAPI] public static SizeValue FromKilobytes(long value) => value * KB;
    [Pure, PublicAPI] public static SizeValue FromMegabytes(long value) => value * MB;
    [Pure, PublicAPI] public static SizeValue FromGigabytes(long value) => value * GB;
    [Pure, PublicAPI] public static SizeValue FromTerabytes(long value) => value * TB;

    [Pure, PublicAPI] public static SizeValue operator *(SizeValue value, long k) => new(value.Bytes * k);
    [Pure, PublicAPI] public static SizeValue operator *(long k, SizeValue value) => new(value.Bytes * k);

    public override string ToString() => ToString(null, null, null);

    [Pure, PublicAPI]
    public string ToString(
        string? format,
        CultureInfo? cultureInfo = null,
        UnitPresentation? unitPresentation = null)
    {
        return ToString(null, format, cultureInfo, unitPresentation);
    }

    [Pure]
    public string ToString(
        SizeUnit? sizeUnit,
        string? format = null,
        CultureInfo? cultureInfo = null,
        UnitPresentation? unitPresentation = null)
    {
        sizeUnit ??= SizeUnit.GetBestSizeUnit(Bytes);
        format ??= DefaultFormat;
        double nominalValue = SizeUnit.Convert(Bytes, SizeUnit.B, sizeUnit);
        var measurementValue = new MeasurementValue(nominalValue, sizeUnit);
        return measurementValue.ToString(format, cultureInfo, unitPresentation);
    }
}