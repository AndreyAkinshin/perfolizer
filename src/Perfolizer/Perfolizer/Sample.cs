using System.Text;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;

namespace Perfolizer;

public class Sample : IFormattableUnit
{
    private const string DefaultFormat = "G";

    public IReadOnlyList<double> Values { get; }
    public IReadOnlyList<double> Weights { get; }
    public double TotalWeight { get; }
    public bool IsWeighted { get; }
    public MeasurementUnit MeasurementUnit { get; }

    private readonly Lazy<(IReadOnlyList<double> SortedValues, IReadOnlyList<double> SortedWeights)> lazySortedData;

    public IReadOnlyList<double> SortedValues => lazySortedData.Value.SortedValues;
    public IReadOnlyList<double> SortedWeights => lazySortedData.Value.SortedWeights;

    /// <summary>
    /// Sample size
    /// </summary>
    public int Count => Values.Count;

    /// <summary>
    /// Kish's Effective Sample Size
    /// </summary>
    public double WeightedCount { get; }

    public Sample(IReadOnlyList<double> values, MeasurementUnit? measurementUnit = null)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);

        Values = values;
        MeasurementUnit = measurementUnit ?? NumberUnit.Instance;
        double weight = 1.0 / values.Count;
        Weights = new IdenticalReadOnlyList<double>(values.Count, weight);
        TotalWeight = 1.0;
        WeightedCount = values.Count;
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
        MeasurementUnit = measurementUnit ?? NumberUnit.Instance;
        TotalWeight = totalWeight;
        WeightedCount = totalWeight.Sqr() / totalWeightSquared;
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

    public Sample(IEnumerable<int> values, MeasurementUnit? measurementUnit = null)
        : this(values.Select(x => (double)x).ToList(), measurementUnit)
    {
    }

    public Sample(IEnumerable<long> values, MeasurementUnit? measurementUnit = null)
        : this(values.Select(x => (double)x).ToList(), measurementUnit)
    {
    }

    private static bool IsSorted(IReadOnlyList<double> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
            if (list[i] > list[i + 1])
                return false;
        return true;
    }

    public static Sample operator *(Sample sample, double value)
    {
        double[] values = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
            values[i] = sample.Values[i] * value;
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }

    public static Sample operator /(Sample sample, double value)
    {
        double[] values = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
            values[i] = sample.Values[i] / value;
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }

    public static Sample operator +(Sample sample, double value)
    {
        double[] values = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
            values[i] = sample.Values[i] + value;
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }

    public static Sample operator -(Sample sample, double value)
    {
        double[] values = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
            values[i] = sample.Values[i] - value;
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }

    public static Sample operator *(Sample sample, int value) => sample * (double)value;
    public static Sample operator /(Sample sample, int value) => sample / (double)value;
    public static Sample operator +(Sample sample, int value) => sample + (double)value;
    public static Sample operator -(Sample sample, int value) => sample - (double)value;

    public override string ToString() => ToString(null);

    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        format ??= DefaultFormat;
        var builder = new StringBuilder();
        builder.Append('[');
        for (int i = 0; i < Values.Count; i++)
        {
            if (i != 0)
                builder.Append(", ");
            builder.Append(Values[i].ToString(format, formatProvider));
        }
        builder.Append(']');
        builder.Append(MeasurementUnit.ToString(unitPresentation));
        return builder.ToString();
    }
}