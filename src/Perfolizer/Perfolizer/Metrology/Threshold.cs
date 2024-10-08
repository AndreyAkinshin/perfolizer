using System.Text;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.GenericEstimators;

namespace Perfolizer.Metrology;

/// <summary>
/// https://aakinshin.net/posts/post-trinal-thresholds/
/// </summary>
public class Threshold(params ISpecificMeasurementValue[] thresholdValues) : IWithUnits, IEquatable<Threshold>
{
    public static readonly Threshold Zero = new ();

    private const char Separator = '|';

    private readonly string presentation = Format(thresholdValues);

    public Sample ApplyMax(Sample sample)
    {
        var appliedSamples = thresholdValues
            .Select(thresholdValue => thresholdValue.Apply(sample))
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

    public double EffectiveShift(Sample sample)
    {
        var basicShifts = thresholdValues
            .OfType<IAbsoluteMeasurementValue>()
            .Select(value => value.GetShift(sample));
        var ratioShifts = thresholdValues
            .OfType<IRelativeMeasurementValue>()
            .Select(value => value.Apply(sample))
            .WhereNotNull()
            .Select(sample2 => HodgesLehmannEstimator.Instance.Shift(sample2, sample));
        return basicShifts.Concat(ratioShifts).DefaultIfEmpty(0).Max();
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
        IReadOnlyList<ISpecificMeasurementValue> thresholdValues,
        string? format = null,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < thresholdValues.Count; i++)
        {
            if (i != 0)
                builder.Append(Separator);
            builder.Append(thresholdValues[i].ToString(format, formatProvider, unitPresentation));
        }
        return builder.ToString();
    }

    public static bool TryParse(string s, out Threshold threshold)
    {
        string[] parts = s.Split(Separator);
        var thresholdValues = new ISpecificMeasurementValue[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!Measurement.TryParse(parts[i], out var measurementValue))
            {
                threshold = Zero;
                return false;
            }

            var applicableMeasurementUnit = measurementValue.AsApplicableMeasurementUnit();
            if (applicableMeasurementUnit == null)
            {
                threshold = Zero;
                return false;
            }

            thresholdValues[i] = applicableMeasurementUnit;
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