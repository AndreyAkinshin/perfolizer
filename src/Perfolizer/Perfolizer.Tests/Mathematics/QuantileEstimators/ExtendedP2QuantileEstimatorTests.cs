using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.DispersionEstimators;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public class ExtendedP2QuantileEstimatorTests
    {
        private readonly ITestOutputHelper output;

        public ExtendedP2QuantileEstimatorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static readonly IDictionary<string, P2QuantileEstimatorTests.TestData> P2TestDataMap = P2QuantileEstimatorTests.TestDataMap;

        [UsedImplicitly] public static TheoryData<string> P2TestDataKeys = P2QuantileEstimatorTests.TestDataKeys;

        [Theory]
        [MemberData(nameof(P2TestDataKeys))]
        public void ExtendedP2IsConsistentWithP2([NotNull] string testKey)
        {
            var testData = P2TestDataMap[testKey];
            double p = testData.Probability;
            var sample = testData.Generate();
            var p2Estimator = new P2QuantileEstimator(p);
            var ep2Estimator = new ExtendedP2QuantileEstimator(p);
            var comparer = new AbsoluteEqualityComparer(1e-9);
            for (int index = 0; index < sample.Values.Count; index++)
            {
                double x = sample.Values[index];
                p2Estimator.Add(x);
                ep2Estimator.Add(x);

                double p2Estimation = p2Estimator.GetQuantile();
                double ep2Estimation = ep2Estimator.GetQuantile(p);
                output.TraceLine("Index : {0}", index);
                output.TraceLine("P2    : {0}", p2Estimation.ToStringInvariant());
                output.TraceLine("ExP2  : {0}", ep2Estimation.ToStringInvariant());
                output.TraceLine("");

                Assert.Equal(p2Estimation, ep2Estimation, comparer);
            }
        }

        [Theory]
        [MemberData(nameof(P2TestDataKeys))]
        public void ExtendedP2QuantileEstimatorTest([NotNull] string testKey)
        {
            var testData = P2TestDataMap[testKey];
            var probabilities = Enumerable
                .Range(1, (int)Math.Round(testData.Probability * 10))
                .Select(x => (Probability)(x / 10.0))
                .ToArray();
            var sample = testData.Generate();
            var estimator = new ExtendedP2QuantileEstimator(probabilities);
            foreach (double x in sample.Values)
                estimator.Add(x);

            foreach (var p in probabilities)
            {
                double actual = estimator.GetQuantile(p);
                double expected = SimpleQuantileEstimator.Instance.GetQuantile(sample, p);
                double pDelta = 0.1 + Math.Abs(p - 0.5) * 0.05 + 0.5 / testData.N;
                double expectedMin = SimpleQuantileEstimator.Instance.GetQuantile(sample, (p - pDelta).Clamp(0, 1));
                double expectedMax = SimpleQuantileEstimator.Instance.GetQuantile(sample, (p + pDelta).Clamp(0, 1));
                double mad = SimpleStdDevConsistentMedianAbsoluteDeviationEstimator.Instance.Calc(sample);
                double error = Math.Abs(actual - expected);
                double errorNorm = error / mad;

                output.WriteLine($"P           = {p:N2}");
                output.WriteLine($"ExpectedMin = {expectedMin:N5}");
                output.WriteLine($"Expected    = {expected:N5}");
                output.WriteLine($"ExpectedMax = {expectedMax:N5}");
                output.WriteLine($"Actual      = {actual:N5}");
                output.WriteLine($"Error       = {error:N5}");
                output.WriteLine($"ErrorNorm   = {errorNorm:N5}");
                output.WriteLine();

                Assert.True(expectedMin <= actual && actual <= expectedMax);
            }
        }
    }
}