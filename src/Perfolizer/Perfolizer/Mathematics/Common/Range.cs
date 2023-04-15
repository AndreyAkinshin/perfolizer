using System.Globalization;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common;

public readonly struct Range
{
    public static Range PositiveInfinity = Of(double.PositiveInfinity, double.PositiveInfinity);
    public static Range NegativeInfinity = Of(double.NegativeInfinity, double.NegativeInfinity);
    public static Range Zero = Of(0, 0);
    public static Range NaN = Of(double.NaN, double.NaN);
        
    private const string DefaultFormat = "N2";

    public double Left { get; }
    public double Right { get; }

    public double Middle => (Left + Right) / 2;

    private Range(double left, double right)
    {
        Left = left;
        Right = right;
    }

    public static Range Of(double left, double right) => new Range(left, right);

    public bool IsInside(Range outerRange)
    {
        return outerRange.Left <= Left && Right <= outerRange.Right;
    }

    public bool ContainsInclusive(double value) => Left <= value && value <= Right;

    public string ToString(CultureInfo? cultureInfo, string? format = "N2")
    {
        cultureInfo ??= DefaultCultureInfo.Instance;
        format ??= DefaultFormat;
        return $"[{Left.ToString(format, cultureInfo)};{Right.ToString(format, cultureInfo)}]";
    }

    public string ToString(string? format) => ToString(null, format);

    public override string ToString() => ToString(null, null);
}