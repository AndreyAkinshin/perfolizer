using System;
using System.Collections.Generic;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public class GreenwaldKhannaQuantileEstimatorTests
    {
        private readonly ITestOutputHelper output;

        public GreenwaldKhannaQuantileEstimatorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(0.1)]
        [InlineData(0.05)]
        [InlineData(0.01)]
        public void GreenwaldKhannaSmokeTest(double eps)
        {
            var probabilities = new Probability[] { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
            var gk = new GreenwaldKhannaQuantileEstimator(eps);
            Assert.Equal(eps, gk.Epsilon);
            var values = new List<double>();
            var random = new Random(1729);
            int maxTupleCount = 0;
            for (int i = 0; i < 1_000; i++)
            {
                double x = random.NextDouble();
                values.Add(x);
                gk.Add(x);
                gk.CheckConsistency();
                output.WriteLine($"n = {gk.Count}, tupleCount = {gk.TupleCount}");
                maxTupleCount = Math.Max(maxTupleCount, gk.TupleCount);
                var sample = new Sample(values);

                
                foreach (var probability in probabilities)
                {
                    int n = sample.Count;
                    double rank = (n - 1) * probability;
                    var margin = Math.Ceiling(n * eps);
                    int leftIndex = Math.Max((int)Math.Floor(rank - margin), 0);
                    double left = sample.SortedValues[leftIndex];
                    int rightIndex = Math.Min((int)Math.Ceiling(rank + margin), n - 1);
                    double right = sample.SortedValues[rightIndex];
                    double estimation = gk.GetQuantile(probability);
                    bool requirementIsSatisfied = left <= estimation && estimation <= right;
                    if (!requirementIsSatisfied)
                    {
                        output.WriteLine(gk.DumpToString());
                        for (int j = 0; j <= i; j++)
                          output.WriteLine("a[" + (j + 1) + "] = " + sample.SortedValues[j]);
                        output.WriteLine("");
                        output.WriteLine("p: " + probability);
                        output.WriteLine("rank: " + (rank + 1));
                        output.WriteLine("margin: " + (margin));
                        output.WriteLine("leftIndex: " + (leftIndex + 1));
                        output.WriteLine("rightIndex: " + (rightIndex + 1));
                        output.WriteLine("left  = " + left);
                        output.WriteLine("est   = " + estimation);
                        output.WriteLine("right = " + right);
                        Assert.True(requirementIsSatisfied);
                    }
                }
            }
            gk.Compress();
            output.WriteLine($"MaxObservedTupleCount = {maxTupleCount}");
            output.WriteLine($"FinalCompressedTupleCount = {gk.TupleCount}");
            output.WriteLine("");
            output.WriteLine(gk.DumpToString());
        }
    }
}