using System;
using System.Linq;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;
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