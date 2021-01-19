using System;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Randomization;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    [UsedImplicitly]
    public class DoubleHeapMovingQuantileEstimatorTests : MovingQuantileEstimatorTestsBase
    {
        public DoubleHeapMovingQuantileEstimatorTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ISequentialQuantileEstimator CreateEstimator(int windowSize, int k,
            MovingQuantileEstimatorInitStrategy initStrategy)
        {
            return new DoubleHeapMovingQuantileEstimator(windowSize, k, initStrategy);
        }

        protected override ISequentialQuantileEstimator CreateEstimator(int windowSize, Probability p)
        {
            return new DoubleHeapMovingQuantileEstimator(windowSize, p);
        }

        [Fact]
        public void HyndmanFanDoubleHeapQuantileEstimatorTest()
        {
            double[] fullSource = {1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66};
            var probabilities = Enumerable.Range(0, 101).Select(x => (Probability) (x / 100.0)).ToArray();
            var types = new[]
            {
                HyndmanFanType.Type1, HyndmanFanType.Type2, HyndmanFanType.Type3, HyndmanFanType.Type4, HyndmanFanType.Type5,
                HyndmanFanType.Type6, HyndmanFanType.Type7, HyndmanFanType.Type8, HyndmanFanType.Type9,
            };

            var comparer = new AbsoluteEqualityComparer(1e-2);
            var shuffler = new Shuffler(new Random(42));

            for (int n = 1; n <= fullSource.Length; n++)
            {
                double[] source = fullSource.Take(n).ToArray();
                var sample = new Sample(source);
                foreach (var type in types)
                    for (int i = 0; i < probabilities.Length; i++)
                    {
                        var probability = probabilities[i];

                        var hyEstimator = new HyndmanFanQuantileEstimator(type);
                        double expectedValue = hyEstimator.GetQuantile(sample, probability);

                        var dhEstimator = new DoubleHeapMovingQuantileEstimator(n, probability, type);
                        shuffler.Shuffle(source);
                        foreach (double value in source)
                            dhEstimator.Add(value);
                        double actualValue = dhEstimator.GetQuantile();

                        if (!comparer.Equals(expectedValue, actualValue))
                            Output.WriteLine($"n = {n}, type = {type}, p = {probability}: E = {expectedValue}, A = {actualValue}");
                        Assert.Equal(expectedValue, actualValue, comparer);
                    }
            }
        }
    }
}