using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Common;

public static class PairwiseEstimatorHelper
{
    public static double Estimate(Sample x, Func<double, double, double> func, IQuantileEstimator estimator, Probability p)
    {
        if (!estimator.SupportsWeightedSamples)
            Assertion.NonWeighted(nameof(x), x);

        int n = x.Count;
        if (x.IsWeighted)
        {
            double[] values = new double[n * (n + 1) / 2];
            double[] weights = new double[n * (n + 1) / 2];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = i; j < n; j++)
            {
                values[k] = func(x.Values[i], x.Values[j]);
                weights[k++] = x.Weights[i] * x.Weights[j];
            }
            return estimator.Quantile(new Sample(values, weights), p);
        }
        else
        {
            double[] values = new double[n * (n + 1) / 2];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = i; j < n; j++)
                values[k++] = func(x.Values[i], x.Values[j]);
            return estimator.Quantile(new Sample(values), p);
        }
    }

    public static double Estimate(Sample x, Sample y, Func<double, double, double> func, IQuantileEstimator estimator, Probability p)
    {
        if (!estimator.SupportsWeightedSamples)
        {
            Assertion.NonWeighted(nameof(x), x);
            Assertion.NonWeighted(nameof(y), y);
        }

        int n = x.Count, m = y.Count;
        if (x.IsWeighted || y.IsWeighted)
        {
            double[] values = new double[n * m];
            double[] weights = new double[n * m];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                values[k] = func(x.Values[j], y.Values[i]);
                weights[k++] = x.Weights[j] * y.Weights[i];
            }
            return estimator.Quantile(new Sample(values, weights), p);
        }
        else
        {
            double[] values = new double[n * m];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                values[k++] = func(x.Values[j], y.Values[i]);
            return estimator.Quantile(new Sample(values), p);
        }
    }
}