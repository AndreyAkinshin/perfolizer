using System;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting
{
    public class StudentTest
    {
        public static readonly StudentTest Instance = new StudentTest();

        /// <summary>
        /// Determines whether the sample mean is different from a known mean
        /// </summary>
        /// <remarks>Should be consistent with t.test(x, mu = mu, alternative = "greater") from R </remarks>
        public OneSidedTestResult IsGreater(double[] sample, double value, Threshold threshold = null)
        {
            var moments = Moments.Create(sample);
            double mean = moments.Mean;
            double stdDev = moments.StandardDeviation;
            double n = sample.Length;
            double df = n - 1;

            threshold = threshold ?? RelativeThreshold.Default;

            double t = (mean - value) /
                       (stdDev / Math.Sqrt(n));
            double pValue = 1 - new StudentDistribution(df).Cdf(t);

            return new OneSidedTestResult(pValue, threshold);
        }
    }
}