using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Perfolizer.Exceptions;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

[SuppressMessage("ReSharper", "InconsistentNaming")] // For using standard notation: "KB", "MB", "GB", "TB"
public readonly struct SizeValue(long bytes)
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

    public Measurement ToMeasurement(SizeUnit? sizeUnit = null)
    {
        sizeUnit ??= SizeUnit.GetBestSizeUnit(Bytes);
        double nominalValue = SizeUnit.Convert(Bytes, SizeUnit.B, sizeUnit);
        return new Measurement(nominalValue, sizeUnit);
    }

    public override string ToString() => MeasurementFormatter.Default.Format(ToMeasurement(), DefaultFormat);
}