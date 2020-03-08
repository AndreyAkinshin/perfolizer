using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public abstract class QuantileEstimatorTests
    {
        private readonly ITestOutputHelper output;

        protected QuantileEstimatorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        protected class TestData
        {
            public double[] Source { get; }
            public double[] Quantiles { get; }
            public double[] Expected { get; }

            public TestData(double[] source, double[] quantiles, double[] expected)
            {
                Source = source;
                Quantiles = quantiles;
                Expected = expected;
            }
        }

        private void DumpArray([NotNull] string name, [NotNull] IEnumerable<double> values)
        {
            string valuesString = string.Join("; ", values.Select(x => x.ToString(TestCultureInfo.Instance)));
            output.WriteLine($"{name}: [{valuesString}]");
        }

        protected void Check([NotNull] IQuantileEstimator estimator, [NotNull] TestData testData)
        {
            var comparer = new AbsoluteEqualityComparer(1e-2);
            DumpArray("Source    ", testData.Source);
            var actual = estimator.GetQuantiles(testData.Source, testData.Quantiles);
            for (int i = 0; i < testData.Quantiles.Length; i++)
            {
                output.WriteLine("-----");
                output.WriteLine("Quantile : " + testData.Quantiles[i]);
                output.WriteLine("Expected : " + testData.Expected[i]);
                output.WriteLine("Actual   : " + actual[i]);
                Assert.Equal(testData.Expected[i], actual[i], comparer);
            }
        }
    }
}