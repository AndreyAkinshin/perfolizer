using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Infra;

namespace Perfolizer.SimulationTests.QuantileEstimators;

public abstract class QuantileEstimatorTests
{
    protected readonly ITestOutputHelper Output;

    protected QuantileEstimatorTests(ITestOutputHelper output)
    {
        Output = output;
    }

    protected class TestData
    {
        public double[] Source { get; }
        public Probability[] Probabilities { get; }
        public double[] Expected { get; }
        public double[]? Weights { get; }

        public TestData(double[] source, Probability[] probabilities, double[] expected, double[]? weights = null)
        {
            Source = source;
            Probabilities = probabilities;
            Expected = expected;
            Weights = weights;
        }
    }

    private void DumpArray(string name, IEnumerable<double> values)
    {
        string valuesString = string.Join("; ", values.Select(x => x.ToString(TestCultureInfo.Instance)));
        Output.WriteLine($"{name}: [{valuesString}]");
    }

    protected void Check(IQuantileEstimator estimator, TestData testData)
    {
        if (testData.Weights == null)
            CheckSimple(testData, estimator.Quantiles(testData.Source, testData.Probabilities), "Non-Weighted");

        if (estimator.SupportsWeightedSamples)
        {
            double[] weights = testData.Weights ?? Enumerable.Range(0, testData.Source.Length).Select(_ => 1.0).ToArray();
            var sample = new Sample(testData.Source, weights);
            CheckSimple(testData, estimator.Quantiles(sample, testData.Probabilities), "Weighted");
        }
    }
        
    protected void CheckSimple(TestData testData, double[] actual, string kind)
    {
        var comparer = new AbsoluteEqualityComparer(1e-2);
        DumpArray("Source    ", testData.Source);
        for (int i = 0; i < testData.Probabilities.Length; i++)
        {
            Output.WriteLine($"----- {kind} -----");
            Output.WriteLine("Probability : " + testData.Probabilities[i]);
            Output.WriteLine("Expected    : " + testData.Expected[i]);
            Output.WriteLine("Actual      : " + actual[i]);
            Assert.Equal(testData.Expected[i], actual[i], comparer);
        }
    }
}