using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;
using Pragmastat;
using Pragmastat.Metrology;

namespace Perfolizer.Mathematics.QuantileEstimators;

/// <summary>
/// This quantile estimator supports nine popular estimation algorithms that are described in [Hyndman1996].
/// <remarks>
/// Hyndman, Rob J., and Yanan Fan. "Sample quantiles in statistical packages." The American Statistician 50, no. 4 (1996): 361-365.
/// https://doi.org/10.2307/2684934
/// </remarks>
/// </summary>
public class HyndmanFanQuantileEstimator : IQuantileEstimator
{
    public static readonly HyndmanFanQuantileEstimator Type1 = new(HyndmanFanType.Type1);
    public static readonly HyndmanFanQuantileEstimator Type2 = new(HyndmanFanType.Type2);
    public static readonly HyndmanFanQuantileEstimator Type3 = new(HyndmanFanType.Type3);
    public static readonly HyndmanFanQuantileEstimator Type4 = new(HyndmanFanType.Type4);
    public static readonly HyndmanFanQuantileEstimator Type5 = new(HyndmanFanType.Type5);
    public static readonly HyndmanFanQuantileEstimator Type6 = new(HyndmanFanType.Type6);
    public static readonly HyndmanFanQuantileEstimator Type7 = new(HyndmanFanType.Type7);
    public static readonly HyndmanFanQuantileEstimator Type8 = new(HyndmanFanType.Type8);
    public static readonly HyndmanFanQuantileEstimator Type9 = new(HyndmanFanType.Type9);

    public HyndmanFanType Type { get; }

    public HyndmanFanQuantileEstimator(HyndmanFanType type)
    {
        if (!Enum.IsDefined(typeof(HyndmanFanType), type))
            throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown type");

        Type = type;
    }

    /// <summary>
    /// Returns 1-based real index estimation
    /// </summary>
    private double GetH(double n, Probability p) => HyndmanFanHelper.GetH(Type, n, p);

    public virtual Measurement Quantile(Sample sample, Probability probability)
    {
        if (!SupportsWeightedSamples)
            Assertion.NonWeighted(nameof(sample), sample);

        return sample.IsWeighted
            ? GetQuantileForWeightedSample(sample, probability).WithUnitOf(sample)
            : GetQuantileForNonWeightedSample(sample, probability).WithUnitOf(sample);
    }

    // See https://aakinshin.net/posts/weighted-quantiles/
    private double GetQuantileForWeightedSample(Sample sample, Probability probability)
    {
        Assertion.NotNull(nameof(sample), sample);

        int n = sample.Size;
        double p = probability;
        double h = GetH(n, p).Clamp(1, n);
        double left = (h - 1) / n;
        double right = h / n;

        double Cdf(double x)
        {
            if (x <= left)
                return 0;
            if (x >= right)
                return 1;
            return x * n - h + 1;
        }

        double totalWeight = sample.TotalWeight;
        double result = 0;
        double current = 0;
        for (int i = 0; i < n; i++)
        {
            double next = current + sample.SortedWeights[i] / totalWeight;
            result += sample.SortedValues[i] * (Cdf(next) - Cdf(current));
            current = next;
        }

        return result;
    }

    private double GetQuantileForNonWeightedSample(Sample sample, Probability probability)
    {
        var sortedValues = sample.SortedValues;

        double GetValue(int index)
        {
            index -= 1; // Adapt one-based formula to the zero-based list
            if (index <= 0)
                return sortedValues[0];
            if (index >= sample.Size)
                return sortedValues[sample.Size - 1];
            return sortedValues[index];
        }

        return HyndmanFanHelper.Evaluate(Type, sample.Size, probability, GetValue);
    }

    public virtual bool SupportsWeightedSamples => HyndmanFanHelper.SupportsWeightedSamples(Type);
    public virtual string Alias => "HF" + (int)Type;
}