using System;
using System.Collections.Generic;
using System.Linq;
using Perfolizer.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.ScaleEstimators;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.ScaleEstimators
{
    public class ScaleEstimatorTestsBase
    {
        private readonly ITestOutputHelper output;

        protected ScaleEstimatorTestsBase(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        public static IEnumerable<object[]> CommonConsistencySampleSizes => new[]
        {
            new object[] { 3 },
            new object[] { 5 },
            new object[] { 10 },
            new object[] { 50 }
        };

        protected void CheckConsistency(int sampleSize, IScaleEstimator scaleEstimator)
        {
            var distribution = NormalDistribution.Standard;
            var randomGenerator = distribution.Random(new Random(42));
            var estimations = new List<double>();
            for (int i = 0; i < 10_000; i++)
            {
                var sample = new Sample(randomGenerator.Next(sampleSize));
                estimations.Add(scaleEstimator.Scale(sample));
            }
            double bias = Math.Abs(estimations.Average() - 1);
            output.WriteLine("Bias = " + bias.ToStringInvariant());
            Assert.True(bias < 0.005);
        }
    }
}