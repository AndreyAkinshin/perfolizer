using System;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Mathematics.EffectSizes;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.EffectSizes
{
    public class GammaEffectSizeTests
    {
        [NotNull] private readonly ITestOutputHelper output;

        public GammaEffectSizeTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(1.0)]
        [InlineData(1.5)]
        [InlineData(2.0)]
        [InlineData(2.5)]
        [InlineData(10.0)]
        public void GammaCohenDConsistencyTest(double stdDev)
        {
            const int n = 100;
            const double delta = 1.0;
            var random = new Random(42);
            var diffs = new double[n];
            output.WriteLine("mean(X)-mean(Y) : " + delta);
            output.WriteLine("StandardDeviation : " + stdDev);

            for (int i = 0; i < n; i++)
            {
                var x = new NormalDistribution(0, 1).Random(random).Next(200);
                var y = new NormalDistribution(delta, 1).Random(random).Next(200);
                var gamma = GammaEffectSize.CalcRange(x, y, 0.5);
                double d = CohenDEffectSize.Calc(x, y);
                diffs[i] = Math.Abs(d - gamma.Middle);
            }

            Array.Sort(diffs);
            output.WriteLine("*** Sorted Abs(Gamma-d) ***");
            for (int i = 0; i < diffs.Length; i++)
                output.WriteLine($"{i}: {diffs[i]:0.0000}");
            output.WriteLine("");

            void Check(int percentile, double expectedMax)
            {
                double diff = diffs[(n - 1) * percentile / 100];
                output.WriteLine($"{percentile}%: {diff:0.0000} (Max = {expectedMax:0.00})");
                Assert.True(diff < expectedMax);
            }

            Check(10, 0.02);
            Check(20, 0.03);
            Check(30, 0.04);
            Check(40, 0.05);
            Check(50, 0.06);
            Check(60, 0.08);
            Check(70, 0.10);
            Check(80, 0.13);
            Check(90, 0.16);
            Check(100, 0.40);
        }

        [Fact]
        public void GammaEffectSizeMultimodalTest()
        {
            var random = new Random(42);
            var x = new NormalDistribution(0, 1).Random(random).Next(200)
                .Concat(new NormalDistribution(10, 1).Random(random).Next(200))
                .ToList();
            var y = new NormalDistribution(0, 1).Random(random).Next(200)
                .Concat(new NormalDistribution(20, 1).Random(random).Next(200))
                .ToList();

            var probabilities = new[] {0.2, 0.3, 0.4, 0.6, 0.7, 0.8};
            var minExpected = new[] {-0.1, -0.1, -0.1, 0.8, 0.8, 0.8};
            var maxExpected = new[] {0.1, 0.1, 0.1, 0.9, 0.9, 0.9};

            for (int i = 0; i < probabilities.Length; i++)
            {
                double p = probabilities[i];
                double gamma = GammaEffectSize.CalcValue(x, y, p);
                output.WriteLine($"{p}: {gamma:0.0000}");
                Assert.True(minExpected[i] < gamma && gamma < maxExpected[i]);
            }
        }
    }
}