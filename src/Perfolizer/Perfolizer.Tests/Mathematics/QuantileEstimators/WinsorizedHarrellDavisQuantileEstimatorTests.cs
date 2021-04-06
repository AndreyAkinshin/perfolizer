using System;
using System.Linq;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public class WinsorizedHarrellDavisQuantileEstimatorTests
    {
        private readonly ITestOutputHelper output;

        public WinsorizedHarrellDavisQuantileEstimatorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void WinsorizedHarrellDavisQuantileEstimatorTest1()
        {
            var randomGenerator = NormalDistribution.Standard.Random(42);
            double[] values = randomGenerator.Next(10);
            values[0] = 1_000_000;
            var sample = values.ToSample();
            var hdEstimator = new HarrellDavisQuantileEstimator();
            var whdEstimator = new WinsorizedHarrellDavisQuantileEstimator(0.01);
            double hdMedian = hdEstimator.GetMedian(sample);
            double whdMedian = whdEstimator.GetMedian(sample);
            output.WriteLine("Median-HD  = " + hdMedian.ToStringInvariant());
            output.WriteLine("Median-WHD = " + whdMedian.ToStringInvariant());

            Assert.True(Math.Abs(whdMedian) < 4);
        }
        
        [Fact]
        public void WinsorizedHarrellDavisQuantileEstimatorTest2()
        {
            var randomGenerator = new UniformDistribution(5, 15).Random(42);
            const int n = 10;
            double[] values1 = randomGenerator.Next(n);
            Array.Sort(values1);
            values1[0] = 0;
            values1[n - 1] = 20;
            
            double[] values2 = new double[n];
            Array.Copy(values1, values2, n);
            values2[0] = values2[1];
            values2[n - 1] = values2[n - 2];
            
            var sample1 = values1.ToSample();
            var sample2 = values2.ToSample();
            var hdEstimator = new HarrellDavisQuantileEstimator();
            var whdEstimator = new WinsorizedHarrellDavisQuantileEstimator(0.01);
            double actual = whdEstimator.GetMedian(sample1);
            double expected = hdEstimator.GetMedian(sample2);
            output.WriteLine("Actual  = " + actual.ToStringInvariant());
            output.WriteLine("Expected = " + expected.ToStringInvariant());

            Assert.Equal(expected, actual, new AbsoluteEqualityComparer(1e-9));
        }
        
        [Fact]
        public void WinsorizedHarrellDavisQuantileEstimatorTest3()
        {
            var randomGenerator = new UniformDistribution(25, 35).Random(42);
            const int n = 19;
            double[] values1 = randomGenerator.Next(n);
            Array.Sort(values1);
            for (int i = 6; i < n; i++)
                values1[i] = 1e10;
            
            double[] values2 = new double[n];
            Array.Copy(values1, values2, n);
            for (int i = 6; i < n; i++)
                values2[i] = values2[5];
            
            var sample1 = values1.ToSample();
            var sample2 = values2.ToSample();
            var hdEstimator = new HarrellDavisQuantileEstimator();
            var whdEstimator = new WinsorizedHarrellDavisQuantileEstimator(0.01);
            double actual = whdEstimator.GetQuantile(sample1, 0.1);
            double expected = hdEstimator.GetQuantile(sample2, 0.1);
            output.WriteLine("Actual  = " + actual.ToStringInvariant());
            output.WriteLine("Expected = " + expected.ToStringInvariant());

            var interval = whdEstimator.GetWinsorizedInterval(n, 0.1);
            output.WriteLine($"WinsorizedInterval = [{interval.left}; {interval.right}]");

            Assert.Equal(expected, actual, new AbsoluteEqualityComparer(1e-9));
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(0.05)]
        public void WinsorizedIntervals(double trimPercent)
        {
            var estimator = new WinsorizedHarrellDavisQuantileEstimator(trimPercent);
            output.WriteLine("| n | winsorized | breakdown |");
            var ns = Enumerable.Range(2, 49).Concat(new[] {100, 500, 1_000, 10_000, 100_000});
            foreach (int n in ns)
            {
                (int l, int r) = estimator.GetWinsorizedInterval(n, Probability.Half);
                int winsorized = n - (r - l + 1);
                double bp = Math.Min(l, n - 1 - r) * 1.0 / n;
                output.WriteLine($"| {n} | {winsorized} | {bp:N4} |");
            }
        }
    }
}