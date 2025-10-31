using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Pragmastat;

namespace Perfolizer.Mathematics.Common;

// The weighted version is based on https://aakinshin.net/posts/whl/
public static class PairwiseEstimatorHelper
{
    public static double Estimate(
        Sample x,
        Func<double, double, double> func,
        IQuantileEstimator estimator,
        Probability p,
        bool includeDiagonal)
    {
        if (!estimator.SupportsWeightedSamples)
            Assertion.NonWeighted(nameof(x), x);

        int n = x.Size;
        int skip = includeDiagonal ? 0 : 1;
        int size = n * (n + 1) / 2 - n * skip;
        if (x.IsWeighted)
        {
            double[] values = new double[size];
            double[] weights = new double[size];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = i + skip; j < n; j++)
            {
                values[k] = func(x.Values[i], x.Values[j]);
                weights[k++] = x.Weights[i] * x.Weights[j];
            }
            return estimator.Quantile(new Sample(values, weights), p);
        }
        else
        {
            double[] values = new double[size];
            int k = 0;
            for (int i = 0; i < n; i++)
            for (int j = i + skip; j < n; j++)
                values[k++] = func(x.Values[i], x.Values[j]);
            return estimator.Quantile(new Sample(values), p);
        }
    }

    public static double Estimate(
        Sample x,
        Sample y,
        Func<double, double, double> func,
        IQuantileEstimator estimator,
        Probability p)
    {
        if (!estimator.SupportsWeightedSamples)
        {
            Assertion.NonWeighted(nameof(x), x);
            Assertion.NonWeighted(nameof(y), y);
        }

        int n = x.Size, m = y.Size;
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
                values[k++] = func(x.Values[i], y.Values[j]);
            return estimator.Quantile(new Sample(values), p);
        }
    }
}