using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;

namespace Perfolizer;

public class Sample : IWithUnits
{
    private const string DefaultFormat = "G";
    private const char OpenBracket = '[';
    private const char CloseBracket = ']';
    private const char Separator = ',';

    public IReadOnlyList<double> Values { get; }
    public IReadOnlyList<double> Weights { get; }
    public double TotalWeight { get; }
    public bool IsWeighted { get; }
    public MeasurementUnit Unit { get; }

    private readonly Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)> lazySortedData;

    public IReadOnlyList<double> SortedValues => lazySortedData.Value.SortedValues;
    public IReadOnlyList<double> SortedWeights => lazySortedData.Value.SortedWeights;

    /// <summary>
    /// Sample size
    /// </summary>
    public int Size => Values.Count;

    /// <summary>
    /// Kish's Effective Sample Size
    /// </summary>
    public double WeightedSize { get; }

    public Sample(params double[] values) : this(values, null)
    {
    }

    public Sample(params int[] values) : this(values, null)
    {
    }

    public Sample(IReadOnlyList<double> values, MeasurementUnit? measurementUnit = null)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);

        Values = values;
        Unit = measurementUnit ?? NumberUnit.Instance;
        double weight = 1.0 / values.Count;
        Weights = new IdenticalReadOnlyList<double>(values.Count, weight);
        TotalWeight = 1.0;
        WeightedSize = values.Count;
        IsWeighted = false;

        lazySortedData = new Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)>(() =>
        {
            if (IsSorted(Values))
                return (Values, Weights);
            return (Values.CopyToArrayAndSort(), Weights);
        });
    }

    public Sample(IReadOnlyList<double> values, IReadOnlyList<double> weights, MeasurementUnit? measurementUnit = null)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);
        Assertion.NotNullOrEmpty(nameof(weights), weights);
        if (values.Count != weights.Count)
            throw new ArgumentException(
                $"{nameof(weights)} should have the same number of elements as {nameof(values)}",
                nameof(weights));

        double totalWeight = 0, maxWeight = double.MinValue, minWeight = double.MaxValue;
        double totalWeightSquared = 0; // Sum of weight squares
        foreach (double weight in weights)
        {
            totalWeight += weight;
            totalWeightSquared += weight.Sqr();
            maxWeight = Max(maxWeight, weight);
            minWeight = Min(minWeight, weight);
        }

        if (minWeight < 0)
            throw new ArgumentOutOfRangeException(nameof(weights),
                $"All weights in {nameof(weights)} should be non-negative");
        if (totalWeight < 1e-9)
            throw new ArgumentException(nameof(weights),
                $"The sum of all elements from {nameof(weights)} should be positive");

        Values = values;
        Weights = weights;
        Unit = measurementUnit ?? NumberUnit.Instance;
        TotalWeight = totalWeight;
        WeightedSize = totalWeight.Sqr() / totalWeightSquared;
        IsWeighted = true;

        lazySortedData = new Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)>(() =>
        {
            if (IsSorted(Values))
                return (Values, Weights);

            double[] sortedValues = Values.CopyToArray();
            double[] sortedWeights = Weights.CopyToArray();
            Array.Sort(sortedValues, sortedWeights);

            return (sortedValues, sortedWeights);
        });
    }

    [PublicAPI]
    public Sample(IEnumerable<int> values, MeasurementUnit? measurementUnit = null)
        : this(values.Select(x => (double)x).ToList(), measurementUnit)
    {
    }

    public Sample(IEnumerable<long> values, MeasurementUnit? measurementUnit = null)
        : this(values.Select(x => (double)x).ToList(), measurementUnit)
    {
    }

    public Sample Concat(Sample sample)
    {
        if (Unit.GetFlavor() != sample.Unit.GetFlavor())
            throw new ArgumentException(
                $"Different measurement unit flavors: " +
                $"{Unit.GetFlavor()} vs. {sample.Unit.GetFlavor()}",
                nameof(sample));

        var unit1 = Unit;
        var unit2 = sample.Unit;
        var unit = unit1.BaseUnits < unit2.BaseUnits ? unit1 : unit2;

        IEnumerable<double> GetValues(Sample s)
        {
            if (unit.BaseUnits == s.Unit.BaseUnits)
                return s.Values;
            double ratio = s.Unit.BaseUnits * 1.0 / unit.BaseUnits;
            return s.Values.Select(x => x * ratio);
        }

        var values1 = GetValues(this);
        var values2 = GetValues(sample);
        var weights1 = Weights;
        var weights2 = sample.Weights;
        return new Sample(values1.Concat(values2).ToList(), weights1.Concat(weights2).ToList(), unit);
    }

    public override string ToString() => ToString(null);

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        var builder = new StringBuilder();
        builder.Append(OpenBracket);
        for (int i = 0; i < Values.Count; i++)
        {
            if (i != 0)
                builder.Append(Separator);
            builder.Append(Values[i].ToString(format, formatProvider));
        }
        builder.Append(CloseBracket);
        builder.Append(Unit.ToString(unitPresentation));
        return builder.ToString();
    }

    [PublicAPI]
    public static bool TryParse(string s, out Sample sample)
    {
        sample = new Sample(0);
        try
        {
            if (s.IndexOf(OpenBracket) != 0 || !s.Contains(CloseBracket))
                return false;
            int openBracketIndex = s.IndexOf(OpenBracket);
            int closeBracketIndex = s.IndexOf(CloseBracket);
            string main = s.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            string[] valueStrings = main.Split(Separator);
            double[] values = new double[valueStrings.Length];
            for (int i = 0; i < valueStrings.Length; i++)
            {
                string valueString = valueStrings[i];
                if (!double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    return false;
                values[i] = value;
            }

            string unitString = s.Substring(closeBracketIndex + 1);
            if (!MeasurementUnit.TryParse(unitString, out var unit))
                return false;

            sample = new Sample(values, unit);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static Sample Parse(string s) =>
        TryParse(s, out var sample) ? sample : throw new FormatException($"Unknown sample: {s}");

    public static Sample operator *(Sample sample, double value)
    {
        double[] values = new double[sample.Size];
        for (int i = 0; i < sample.Size; i++)
            values[i] = sample.Values[i] * value;
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }

    public static Sample operator /(Sample sample, double value)
    {
        double[] values = new double[sample.Size];
        for (int i = 0; i < sample.Size; i++)
            values[i] = sample.Values[i] / value;
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }

    public static Sample operator +(Sample sample, double value)
    {
        double[] values = new double[sample.Size];
        for (int i = 0; i < sample.Size; i++)
            values[i] = sample.Values[i] + value;
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }

    public static Sample operator -(Sample sample, double value)
    {
        double[] values = new double[sample.Size];
        for (int i = 0; i < sample.Size; i++)
            values[i] = sample.Values[i] - value;
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }

    public static Sample operator *(Sample sample, int value) => sample * (double)value;
    public static Sample operator /(Sample sample, int value) => sample / (double)value;
    public static Sample operator +(Sample sample, int value) => sample + (double)value;
    public static Sample operator -(Sample sample, int value) => sample - (double)value;

    private static bool IsSorted(IReadOnlyList<double> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
            if (list[i] > list[i + 1])
                return false;
        return true;
    }
}