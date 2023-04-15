using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators;

public static class QuantileEstimatorExtensions
{
    public static double[] Quantiles(
        this IQuantileEstimator estimator,
        Sample sample,
        IReadOnlyList<Probability> probabilities)
    {
        Assertion.NotNull(nameof(estimator), estimator);
        Assertion.NotNull(nameof(sample), sample);
        Assertion.NotNull(nameof(probabilities), probabilities);

        double[] results = new double[probabilities.Count];
        for (int i = 0; i < probabilities.Count; i++)
            results[i] = estimator.Quantile(sample, probabilities[i]);

        return results;
    }

    public static double Median(
        this IQuantileEstimator estimator,
        Sample sample)
    {
        return estimator.Quantile(sample, 0.5);
    }

    public static double Quantile(
        this IQuantileEstimator estimator,
        IReadOnlyList<double> values,
        Probability probability)
    {
        return estimator.Quantile(new Sample(values), probability);
    }

    public static double[] Quantiles(
        this IQuantileEstimator estimator,
        IReadOnlyList<double> values,
        IReadOnlyList<Probability> probabilities)
    {
        return estimator.Quantiles(new Sample(values), probabilities);
    }
}