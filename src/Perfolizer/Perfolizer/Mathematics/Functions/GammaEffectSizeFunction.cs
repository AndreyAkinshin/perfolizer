using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.ScaleEstimators;
using Perfolizer.Mathematics.Sequences;

namespace Perfolizer.Mathematics.Functions;

public class GammaEffectSizeFunction : QuantileCompareFunction
{
    private static readonly MedianAbsoluteDeviationEstimator DefaultMedianAbsoluteDeviationEstimator =
        MedianAbsoluteDeviationEstimator.Simple;

    public static readonly GammaEffectSizeFunction Instance = new GammaEffectSizeFunction();

    private const double Eps = 1e-9;

    private readonly MedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator;

    public GammaEffectSizeFunction(MedianAbsoluteDeviationEstimator? medianAbsoluteDeviationEstimator = null) :
        base((medianAbsoluteDeviationEstimator ?? DefaultMedianAbsoluteDeviationEstimator).QuantileEstimator)
    {
        this.medianAbsoluteDeviationEstimator = medianAbsoluteDeviationEstimator ?? DefaultMedianAbsoluteDeviationEstimator;
    }

    public override double Value(Sample a, Sample b, Probability probability)
    {
        Assertion.NotNull(nameof(a), a);
        Assertion.NotNull(nameof(b), b);

        try
        {
            double aMad = medianAbsoluteDeviationEstimator.Mad(a);
            double bMad = medianAbsoluteDeviationEstimator.Mad(b);
            if (aMad < Eps && bMad < Eps)
            {
                double aMedian = QuantileEstimator.Median(a);
                double bMedian = QuantileEstimator.Median(b);
                if (Math.Abs(aMedian - bMedian) < Eps)
                    return 0;
                return aMedian < bMedian
                    ? double.PositiveInfinity
                    : double.NegativeInfinity;
            }

            double aQuantile = QuantileEstimator.Quantile(a, probability);
            double bQuantile = QuantileEstimator.Quantile(b, probability);
            double pooledMad = Pooled(a.Count, b.Count, aMad, bMad);

            return (bQuantile - aQuantile) / pooledMad;
        }
        catch (Exception)
        {
            return double.NaN;
        }
    }

    public override double[] Values(Sample a, Sample b, IReadOnlyList<Probability> probabilities)
    {
        Assertion.NotNull(nameof(a), a);
        Assertion.NotNull(nameof(b), b);
        Assertion.NotNullOrEmpty(nameof(probabilities), probabilities);

        int k = probabilities.Count;
        try
        {
            double aMad = medianAbsoluteDeviationEstimator.Mad(a);
            double bMad = medianAbsoluteDeviationEstimator.Mad(b);
            if (aMad < Eps && bMad < Eps)
            {
                double aMedian = QuantileEstimator.Median(a);
                double bMedian = QuantileEstimator.Median(b);
                if (Math.Abs(aMedian - bMedian) < Eps)
                    return ConstantSequence.Zero.GenerateArray(k);
                return aMedian < bMedian
                    ? ConstantSequence.PositiveInfinity.GenerateArray(k)
                    : ConstantSequence.NegativeInfinity.GenerateArray(k);
            }

            double[] aQuantile = QuantileEstimator.Quantiles(a, probabilities);
            double[] bQuantile = QuantileEstimator.Quantiles(b, probabilities);

            double pooledMad = Pooled(a.Count, b.Count, aMad, bMad);

            double[] values = new double[k];
            for (int i = 0; i < k; i++)
                values[i] = (bQuantile[i] - aQuantile[i]) / pooledMad;

            return values;
        }
        catch (Exception)
        {
            return ConstantSequence.NaN.GenerateArray(k);
        }
    }

    public static double Pooled(int n1, int n2, double value1, double value2)
    {
        return Math.Sqrt(((n1 - 1) * value1.Sqr() + (n2 - 1) * value2.Sqr()) / (n1 + n2 - 2));
    }

    protected override double CalculateValue(double quantileA, double quantileB)
    {
        throw new NotSupportedException();
    }
}