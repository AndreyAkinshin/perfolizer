using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Perfolizer.Exceptions;

namespace Perfolizer.Metrology;

[SuppressMessage("ReSharper", "InconsistentNaming")] // We want to use "KB", "MB", "GB", "TB"
public readonly struct SizeValue(long bytes) : IAbsoluteMeasurementValue
{
    private const string DefaultFormat = "0.##";

    [PublicAPI]
    public long Bytes { get; } = bytes;

    public SizeValue(long nominalValue, SizeUnit unit) : this(nominalValue * unit.BaseUnits)
    {
    }

    [PublicAPI] public static readonly SizeValue B = SizeUnit.B.ToValue();
    [PublicAPI] public static readonly SizeValue KB = SizeUnit.KB.ToValue();
    [PublicAPI] public static readonly SizeValue MB = SizeUnit.MB.ToValue();
    [PublicAPI] public static readonly SizeValue GB = SizeUnit.GB.ToValue();
    [PublicAPI] public static readonly SizeValue TB = SizeUnit.TB.ToValue();

    [PublicAPI] public static SizeValue FromBytes(long value) => value * B;
    [PublicAPI] public static SizeValue FromKilobytes(long value) => value * KB;
    [PublicAPI] public static SizeValue FromMegabytes(long value) => value * MB;
    [PublicAPI] public static SizeValue FromGigabytes(long value) => value * GB;
    [PublicAPI] public static SizeValue FromTerabytes(long value) => value * TB;

    [PublicAPI] public static SizeValue operator *(SizeValue value, long k) => new(value.Bytes * k);
    [PublicAPI] public static SizeValue operator *(long k, SizeValue value) => new(value.Bytes * k);

    public override string ToString() => ToString(null, null, null);

    [PublicAPI]
    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        return ToString(null, format, formatProvider, unitPresentation);
    }

    [PublicAPI]
    public string ToString(
        SizeUnit? sizeUnit,
        string? format = null,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        sizeUnit ??= SizeUnit.GetBestSizeUnit(Bytes);
        format ??= DefaultFormat;
        double nominalValue = SizeUnit.Convert(Bytes, SizeUnit.B, sizeUnit);
        var measurementValue = new Measurement(nominalValue, sizeUnit);
        return measurementValue.ToString(format, formatProvider, unitPresentation);
    }

    public MeasurementUnit Unit => SizeUnit.B;

    public double GetShift(Sample sample)
    {
        if (sample.Unit is not SizeUnit sizeUnit)
            throw new InvalidMeasurementUnitExceptions(Unit, sample.Unit);
        return SizeUnit.Convert(Bytes, SizeUnit.B, sizeUnit);
    }
}