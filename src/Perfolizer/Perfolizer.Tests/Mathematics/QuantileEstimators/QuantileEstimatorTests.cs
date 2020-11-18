using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
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
            [NotNull] public double[] Source { get; }
            [NotNull] public double[] Quantiles { get; }
            [NotNull] public double[] Expected { get; }
            [CanBeNull] public double[] Weights { get; }

            public TestData(double[] source, double[] quantiles, double[] expected, double[] weights = null)
            {
                Source = source;
                Quantiles = quantiles;
                Expected = expected;
                Weights = weights;
            }
        }

        private void DumpArray([NotNull] string name, [NotNull] IEnumerable<double> values)
        {
            string valuesString = string.Join("; ", values.Select(x => x.ToString(TestCultureInfo.Instance)));
            output.WriteLine($"{name}: [{valuesString}]");
        }

        protected void Check([NotNull] IQuantileEstimator estimator, [NotNull] TestData testData)
        {
            if (testData.Weights == null)
                CheckSimple(testData, estimator.GetQuantiles(testData.Source, testData.Quantiles), "Non-Weighted");

            if (estimator.SupportsWeightedSamples)
            {
                double[] weights = testData.Weights ?? Enumerable.Range(0, testData.Source.Length).Select(_ => 1.0).ToArray();
                var sample = new Sample(testData.Source, weights);
                CheckSimple(testData, estimator.GetQuantiles(sample, testData.Quantiles), "Weighted");
            }
        }
        
        protected void CheckSimple([NotNull] TestData testData, [NotNull] double[] actual, string kind)
        {
            var comparer = new AbsoluteEqualityComparer(1e-2);
            DumpArray("Source    ", testData.Source);
            for (int i = 0; i < testData.Quantiles.Length; i++)
            {
                output.WriteLine($"----- {kind} -----");
                output.WriteLine("Quantile : " + testData.Quantiles[i]);
                output.WriteLine("Expected : " + testData.Expected[i]);
                output.WriteLine("Actual   : " + actual[i]);
                Assert.Equal(testData.Expected[i], actual[i], comparer);
            }
        }
    }
}