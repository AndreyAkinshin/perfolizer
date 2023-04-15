using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.GenericEstimators
{
    /// <summary>
    /// Based on:
    /// Hodges, J. L., and E. L. Lehmann. 1963. Estimates of location based on rank tests.
    /// The Annals of Mathematical Statistics 34 (2):598–611.  
    /// DOI: 10.1214/aoms/1177704172
    /// </summary>
    public class HodgesLehmannEstimator : ILocationShiftEstimator, IMedianEstimator
    {
        public static readonly HodgesLehmannEstimator Instance = new();

        public double LocationShift(Sample a, Sample b)
        {
            double[] diffs = new double[a.Count * b.Count];
            int k = 0;
            for (int i = 0; i < a.Count; i++)
            for (int j = 0; j < b.Count; j++)
                diffs[k++] = a.Values[j] - b.Values[i];
            return SimpleQuantileEstimator.Instance.Median(new Sample(diffs));
        }

        public double Median(Sample sample)
        {
            int n = sample.Count;
            double[] diffs = new double[n * (n + 1) / 2];
            for (int i = 0, k = 0; i < n; i++)
            for (int j = i; j < n; j++)
                diffs[k++] = (sample.Values[i] + sample.Values[j]) / 2;
            return SimpleQuantileEstimator.Instance.Median(new Sample(diffs));
        }
    }
}