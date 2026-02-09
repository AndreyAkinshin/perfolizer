using System.Text;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Horology;
using Perfolizer.Mathematics.Common;
using Pragmastat;
using Pragmastat.Estimators;
using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

/// <summary>
/// https://aakinshin.net/posts/post-trinal-thresholds/
/// </summary>
public class Threshold(params Measurement[] thresholdValues) : IEquatable<Threshold>
{
    public static readonly Threshold Zero = new();

    private const char Separator = '|';

    private readonly string presentation = Format(thresholdValues);

    public static Sample Apply(Sample sample, Measurement measurement)
    {
        double m = measurement.NominalValue;
        switch (measurement.Unit)
        {
            case NumberUnit:
                return new Sample(sample.Values.Select(x => x + m).ToArray(), sample.Unit);
            case TimeUnit unit:
                {
                    if (sample.Unit is not TimeUnit timeUnit)
                        throw new InvalidOperationException(
                            $"Can't apply {unit.AbbreviationAscii} to " +
                            $"sample of {sample.Unit.AbbreviationAscii} values");
                    double shift = TimeUnit.Convert(m, unit, timeUnit);
                    return new Sample(sample.Values.Select(x => x + shift).ToArray(), sample.Unit);
                }
            case FrequencyUnit unit:
                {
                    if (sample.Unit is not FrequencyUnit frequencyUnit)
                        throw new InvalidOperationException(
                            $"Can't apply {unit.AbbreviationAscii} to " +
                            $"sample of {sample.Unit.AbbreviationAscii} values");
                    double shift = FrequencyUnit.Convert(m, unit, frequencyUnit);
                    return new Sample(sample.Values.Select(x => x + shift).ToArray(), sample.Unit);
                }
            case SizeUnit unit:
                {
                    if (sample.Unit is not SizeUnit sizeUnit)
                        throw new InvalidOperationException(
                            $"Can't apply {unit.AbbreviationAscii} to " +
                            $"sample of {sample.Unit.AbbreviationAscii} values");
                    double shift = SizeUnit.Convert(m.RoundToLong(), unit, sizeUnit);
                    return new Sample(sample.Values.Select(x => x + shift).ToArray(), sample.Unit);
                }

            case RatioUnit:
                return new Sample(sample.Values.Select(x => x * m).ToArray(), sample.Unit);
            case PercentUnit:
                return new Sample(sample.Values.Select(x => x * (1 + m / 100.0)).ToArray(), sample.Unit);
            // TODO: support DisparityUnit
            default:
                throw new NotSupportedException($"{measurement.Unit.AbbreviationAscii} is not supported");
        }
    }


    public Sample ApplyMax(Sample sample)
    {
        var appliedSamples = thresholdValues
            .Select(thresholdValue => Apply(sample, thresholdValue))
            .WhereNotNull()
            .ToArray();

        double[] values = new double[sample.Size];
        for (int i = 0; i < sample.Size; i++)
        {
            values[i] = double.MinValue;
            foreach (var appliedSample in appliedSamples)
                values[i] = Max(values[i], appliedSample.Values[i]);
            if (appliedSamples.Length == 0)
                values[i] = sample.Values[i];
        }
        return sample.IsWeighted
            ? new Sample(values, sample.Weights, sample.Unit)
            : new Sample(values, sample.Unit);
    }

    // TODO: rework
    public double EffectiveShift(Sample sample)
    {
        return thresholdValues
            .Select(thresholdValue =>
                ShiftEstimator.Instance.Estimate(sample, Apply(sample, thresholdValue)).NominalValue)
            .DefaultIfEmpty(0)
            .Max();
    }

    public override string ToString() => presentation;
    public MeasurementUnit Unit => throw new NotSupportedException("Threshold does not have a measurement unit");

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        return Format(thresholdValues, format, formatProvider, unitPresentation);
    }

    private static string Format(
        IReadOnlyList<Measurement> thresholdValues,
        string? format = null,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < thresholdValues.Count; i++)
        {
            if (i != 0)
                builder.Append(Separator);
            builder.Append(MeasurementFormatter.Default.Format(
                thresholdValues[i], format, formatProvider, unitPresentation));
        }
        return builder.ToString();
    }

    public static bool TryParse(string s, out Threshold threshold)
    {
        string[] parts = s.Split(Separator);
        var thresholdValues = new Measurement[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!PerfolizerMeasurementFormatter.Instance.TryParse(parts[i], out var measurement))
            {
                threshold = Zero;
                return false;
            }

            thresholdValues[i] = measurement;
        }
        threshold = new Threshold(thresholdValues);
        return true;
    }

    [PublicAPI]
    public static Threshold Parse(string s)
    {
        if (!TryParse(s, out var threshold))
            throw new FormatException($"The string '{s}' is not a valid threshold");
        return threshold;
    }

    public bool Equals(Threshold? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return presentation.Equals(other.presentation);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Threshold)obj);
    }

    public override int GetHashCode() => presentation.GetHashCode();
}