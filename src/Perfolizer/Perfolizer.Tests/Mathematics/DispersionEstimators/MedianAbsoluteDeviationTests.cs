using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.DispersionEstimators;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.DispersionEstimators
{
    public class MedianAbsoluteDeviationTests
    {
        [NotNull] private readonly ITestOutputHelper output;

        public MedianAbsoluteDeviationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(50)]
        public void SimpleEstimatorConsistencyTest(int sampleSize)
        {
            Check(sampleSize, SimpleNormalizedMedianAbsoluteDeviationEstimator.Instance);
        }
        
        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(50)]
        public void HdEstimatorConsistencyTest(int sampleSize)
        {
            Check(sampleSize, HarrellDavisNormalizedMedianAbsoluteDeviationEstimator.Instance);
        }

        private void Check(int sampleSize, IMedianAbsoluteDeviationEstimator madEstimator)
        {
            var distribution = NormalDistribution.Standard;
            var randomGenerator = distribution.Random(new Random(42));
            var estimations = new List<double>();
            for (int i = 0; i < 10_000; i++)
            {
                var sample = new Sample(randomGenerator.Next(sampleSize));
                estimations.Add(madEstimator.Calc(sample));
            }
            double bias = Math.Abs(estimations.Average() - 1);
            output.WriteLine("Bias = " + bias.ToStringInvariant());
            Assert.True(bias < 0.005);
        }
    }
}