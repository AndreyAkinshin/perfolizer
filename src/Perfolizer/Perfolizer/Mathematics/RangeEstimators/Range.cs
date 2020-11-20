using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.RangeEstimators
{
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

        [NotNull]
        public string ToString([CanBeNull] CultureInfo cultureInfo, [CanBeNull] string format = "N2")
        {
            cultureInfo ??= DefaultCultureInfo.Instance;
            format ??= DefaultFormat;
            return $"[{Left.ToString(format, cultureInfo)};{Right.ToString(format, cultureInfo)}]";
        }

        [NotNull]
        public string ToString([CanBeNull] string format) => ToString(null, format);

        [NotNull]
        public override string ToString() => ToString(null, null);
    }
}