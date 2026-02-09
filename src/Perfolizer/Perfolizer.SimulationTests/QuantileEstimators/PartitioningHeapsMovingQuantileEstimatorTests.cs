using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Infra;
using Pragmastat;
using Pragmastat.Randomization;

namespace Perfolizer.SimulationTests.QuantileEstimators;

[UsedImplicitly]
public class PartitioningHeapsMovingQuantileEstimatorTests : MovingQuantileEstimatorTestsBase
{
    public PartitioningHeapsMovingQuantileEstimatorTests(ITestOutputHelper output) : base(output)
    {
    }

    protected override ISequentialSpecificQuantileEstimator CreateEstimator(int windowSize, int k,
        MovingQuantileEstimatorInitStrategy initStrategy)
    {
        return new PartitioningHeapsMovingQuantileEstimator(k, windowSize, initStrategy);
    }

    protected override ISequentialSpecificQuantileEstimator CreateEstimator(int windowSize, Probability p)
    {
        return new PartitioningHeapsMovingQuantileEstimator(p, windowSize);
    }

    [Fact]
    public void HyndmanFanPartitioningHeapsQuantileEstimatorTest()
    {
        double[] fullSource = { 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66 };
        var probabilities = Enumerable.Range(0, 101).Select(x => (Probability)(x / 100.0)).ToArray();
        var types = HyndmanFanHelper.AllTypes;

        var comparer = new AbsoluteEqualityComparer(1e-2);
        var rng = new Rng(42);

        for (int n = 1; n <= fullSource.Length; n++)
        {
            double[] source = fullSource.Take(n).ToArray();
            var sample = new Sample(source.CopyToArray());
            foreach (var type in types)
                for (int i = 0; i < probabilities.Length; i++)
                {
                    var probability = probabilities[i];

                    var hyEstimator = new HyndmanFanQuantileEstimator(type);
                    double expectedValue = hyEstimator.Quantile(sample, probability);

                    var phEstimator = new PartitioningHeapsMovingQuantileEstimator(probability, n, type);
                    var shuffled = rng.Shuffle(source);
                    foreach (double value in shuffled)
                        phEstimator.Add(value);
                    double actualValue = phEstimator.Quantile();

                    if (!comparer.Equals(expectedValue, actualValue))
                        Output.WriteLine($"n = {n}, type = {type}, p = {probability}: E = {expectedValue}, A = {actualValue}");
                    Assert.Equal(expectedValue, actualValue, comparer);
                }
        }
    }
}