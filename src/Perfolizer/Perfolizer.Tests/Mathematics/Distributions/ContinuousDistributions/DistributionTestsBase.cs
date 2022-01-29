using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Distributions.ContinuousDistributions
{
    public class DistributionTestsBase
    {
        protected virtual double Eps => 1e-5;
        private readonly IEqualityComparer<double> equalityComparer;

        private readonly ITestOutputHelper output;

        protected DistributionTestsBase(ITestOutputHelper output)
        {
            this.output = output;
            equalityComparer = new AbsoluteEqualityComparer(Eps);
        }

        protected static double[] DefaultProbs = new double[]
        {
            0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1,
            0.11, 0.12, 0.13, 0.14, 0.15, 0.16, 0.17, 0.18, 0.19, 0.2, 0.21,
            0.22, 0.23, 0.24, 0.25, 0.26, 0.27, 0.28, 0.29, 0.3, 0.31, 0.32,
            0.33, 0.34, 0.35, 0.36, 0.37, 0.38, 0.39, 0.4, 0.41, 0.42, 0.43,
            0.44, 0.45, 0.46, 0.47, 0.48, 0.49, 0.5, 0.51, 0.52, 0.53, 0.54,
            0.55, 0.56, 0.57, 0.58, 0.59, 0.6, 0.61, 0.62, 0.63, 0.64, 0.65,
            0.66, 0.67, 0.68, 0.69, 0.7, 0.71, 0.72, 0.73, 0.74, 0.75, 0.76,
            0.77, 0.78, 0.79, 0.8, 0.81, 0.82, 0.83, 0.84, 0.85, 0.86, 0.87,
            0.88, 0.89, 0.9, 0.91, 0.92, 0.93, 0.94, 0.95, 0.96, 0.97, 0.98,
            0.99, 1
        };

        protected class TestData
        {
            public IContinuousDistribution Distribution { get; }
            public double[] X { get; }
            public double[] ExpectedPdf { get; }
            public double[] ExpectedCdf { get; }
            public double[] P { get; }
            public double[] ExpectedQuantiles { get; }

            public TestData(IContinuousDistribution distribution,
                double[] x,
                double[] expectedPdf,
                double[] expectedCdf,
                double[] p,
                double[] expectedQuantiles)
            {
                Distribution = distribution;
                X = x;
                ExpectedPdf = expectedPdf;
                ExpectedCdf = expectedCdf;
                P = p;
                ExpectedQuantiles = expectedQuantiles;
            }
        }

        [AssertionMethod]
        protected void Check(
            TestData data,
            bool skipStdDev = false,
            bool skipPdf = false,
            bool skipCdf = false,
            bool skipQuantile = false)
        {
            output.WriteLine();
            output.WriteLine($"*** {data.Distribution} *** ");

            if (!skipStdDev)
                AssertEqual("StandardDeviation", data.Distribution.StandardDeviation, data.Distribution.Variance.Sqrt());

            if (!skipPdf)
            {
                output.WriteLine("---");
                for (int i = 0; i < data.X.Length; i++)
                    AssertEqual($"Pdf({data.X[i]})", data.ExpectedPdf[i], data.Distribution.Pdf(data.X[i]));
            }

            if (!skipCdf)
            {
                output.WriteLine("---");
                for (int i = 0; i < data.X.Length; i++)
                    AssertEqual($"Cdf({data.X[i]})", data.ExpectedCdf[i], data.Distribution.Cdf(data.X[i]));
            }

            if (!skipQuantile)
            {
                output.WriteLine("---");
                for (int i = 0; i < data.P.Length; i++)
                    AssertEqual($"Quantile({data.P[i]})", data.ExpectedQuantiles[i], data.Distribution.Quantile(data.P[i]));

                Assert.Throws<ArgumentOutOfRangeException>(() => data.Distribution.Quantile(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => data.Distribution.Quantile(2));
            }
        }

        [AssertionMethod]
        protected void AssertEqual(string name, double expected, double actual)
        {
            output.WriteLine($"{name}:");
            output.WriteLine($"  Expected = {expected}");
            output.WriteLine($"  Actual   = {actual}");
            Assert.Equal(expected, actual, equalityComparer);
        }
    }
}