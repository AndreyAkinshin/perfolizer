using System.Text;
using Perfolizer.Collections;

namespace Perfolizer.Metrology;

/// <summary>
/// https://aakinshin.net/posts/trinal-thresholds/
/// </summary>
public class Threshold(params IApplicableMeasurementUnit[] thresholdValues) : IFormattableUnit
{
    public static readonly Threshold Zero = new();

    private const char Separator = '|';

    public Sample ApplyMax(Sample sample)
    {
        var appliedSamples = thresholdValues
            .Select(thresholdValue => thresholdValue.Apply(sample))
            .WhereNotNull()
            .ToArray();

        double[] values = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
        {
            values[i] = double.MinValue;
            foreach (var appliedSample in appliedSamples)
                values[i] = Max(values[i], appliedSample.Values[i]);
            if (appliedSamples.IsEmpty())
                values[i] = sample.Values[i];
        }
        return sample.IsWeighted
            ? new Sample(values, sample.Weights, sample.MeasurementUnit)
            : new Sample(values, sample.MeasurementUnit);
    }


    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < thresholdValues.Length; i++)
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
        var thresholdValues = new IApplicableMeasurementUnit[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!MeasurementValue.TryParse(parts[i], out var measurementValue))
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
}