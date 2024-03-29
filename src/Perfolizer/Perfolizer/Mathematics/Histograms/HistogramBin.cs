﻿using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Horology;

namespace Perfolizer.Mathematics.Histograms;

public class HistogramBin
{
    public double Lower { get; }
    public double Upper { get; }
    public double[] Values { get; }

    public int Count => Values.Length;
    public double Gap => Upper - Lower;
    public bool IsEmpty => Count == 0;
    public bool HasAny => Count > 0;

    public HistogramBin(double lower, double upper, double[] values)
    {
        Lower = lower;
        Upper = upper;
        Values = values;
    }

    public static HistogramBin Union(HistogramBin bin1, HistogramBin bin2) => new HistogramBin(
        Math.Min(bin1.Lower, bin2.Lower),
        Math.Max(bin1.Upper, bin2.Upper),
        bin1.Values.Concat(bin2.Values).OrderBy(value => value).ToArray());

    public override string ToString() => ToString(DefaultCultureInfo.Instance);

    [PublicAPI] public string ToString(CultureInfo cultureInfo)
    {
        var unit = TimeUnit.GetBestTimeUnit(Values);
        var builder = new StringBuilder();
        builder.Append('[');
        builder.Append(TimeInterval.FromNanoseconds(Lower).ToString(unit, formatProvider: cultureInfo));
        builder.Append(';');
        builder.Append(TimeInterval.FromNanoseconds(Upper).ToString(unit, formatProvider: cultureInfo));
        builder.Append(' ');
        builder.Append('{');
        for (var i = 0; i < Values.Length; i++)
        {
            if (i != 0)
                builder.Append("; ");
            builder.Append(TimeInterval.FromNanoseconds(Values[i]).ToString(unit, formatProvider: cultureInfo));
        }
        builder.Append('}');
            
        return builder.ToString();
    }
}