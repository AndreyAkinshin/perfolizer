using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Perfolizer.Common;
using Perfolizer.Horology;
using Perfolizer.Mathematics.SignificanceTesting;

namespace Perfolizer.Mathematics.Thresholds
{
    public abstract class Threshold
    {
        public static Threshold Create(ThresholdUnit unit, double value)
        {
            switch (unit)
            {
                case ThresholdUnit.Ratio: return new RelativeThreshold(value);
                case ThresholdUnit.Nanoseconds: return new AbsoluteTimeThreshold(TimeInterval.FromNanoseconds(value));
                case ThresholdUnit.Microseconds: return new AbsoluteTimeThreshold(TimeInterval.FromMicroseconds(value));
                case ThresholdUnit.Milliseconds: return new AbsoluteTimeThreshold(TimeInterval.FromMilliseconds(value));
                case ThresholdUnit.Seconds: return new AbsoluteTimeThreshold(TimeInterval.FromSeconds(value));
                case ThresholdUnit.Minutes: return new AbsoluteTimeThreshold(TimeInterval.FromMinutes(value));
                default: throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }

        public abstract double GetValue(IReadOnlyList<double> values);
        public abstract bool IsZero();

        public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out Threshold? parsed)
        {
            return TryParse(input, NumberStyles.Any, DefaultCultureInfo.Instance, out parsed);
        }

        public static bool TryParse([NotNullWhen(true)] string? input, NumberStyles numberStyle, IFormatProvider formatProvider, [NotNullWhen(true)] out Threshold? parsed)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                parsed = default;
                return false;
            }

            string trimmed = input.Trim().ToLowerInvariant();
            string number = new string(trimmed.TakeWhile(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
            string unit = new string(trimmed.SkipWhile(c => char.IsDigit(c) || c == '.' || c == ',' || char.IsWhiteSpace(c)).ToArray());

            if (!double.TryParse(number, numberStyle, formatProvider, out var parsedValue)
                || !ThresholdUnitExtensions.ShortNameToUnit.TryGetValue(unit, out var parsedUnit))
            {
                parsed = default;
                return false;
            }

            parsed = parsedUnit == ThresholdUnit.Ratio ? Create(ThresholdUnit.Ratio, parsedValue / 100.0) : Create(parsedUnit, parsedValue);

            return true;
        }
    }
}