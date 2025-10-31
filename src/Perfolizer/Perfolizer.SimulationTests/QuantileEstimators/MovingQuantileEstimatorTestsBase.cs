using System.Text;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Infra;
using Pragmastat;

namespace Perfolizer.SimulationTests.QuantileEstimators;

public abstract class MovingQuantileEstimatorTestsBase
{
    private static bool DiagnosticsMode = false;
    protected ITestOutputHelper Output { get; }

    protected MovingQuantileEstimatorTestsBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected abstract ISequentialSpecificQuantileEstimator CreateEstimator(int windowSize, int k,
        MovingQuantileEstimatorInitStrategy initStrategy);

    protected abstract ISequentialSpecificQuantileEstimator CreateEstimator(int windowSize, Probability p);

    [Theory]
    [InlineData(300, 5, 0)]
    [InlineData(300, 5, 1)]
    [InlineData(300, 5, 2)]
    [InlineData(300, 5, 3)]
    [InlineData(300, 5, 4)]
    [InlineData(300, 127, 62)]
    [InlineData(300, 127, 63)]
    [InlineData(300, 127, 64)]
    [InlineData(300, 127, 65)]
    [InlineData(300, 127, 66)]
    [InlineData(300, 128, 62)]
    [InlineData(300, 128, 63)]
    [InlineData(300, 128, 64)]
    [InlineData(300, 128, 65)]
    [InlineData(300, 128, 66)]
    [InlineData(300, 129, 62)]
    [InlineData(300, 129, 63)]
    [InlineData(300, 129, 64)]
    [InlineData(300, 129, 65)]
    [InlineData(300, 129, 66)]
    [InlineData(10_000, 1023, 511)]
    public void MovingSelectorTest(int totalElementCount, int windowSize, int k)
    {
        var random = new Random(42);
        foreach (var initStrategy in new[]
                     {MovingQuantileEstimatorInitStrategy.OrderStatistics, MovingQuantileEstimatorInitStrategy.QuantileApproximation})
        {
            Output.WriteLine($"*** {nameof(MovingQuantileEstimatorInitStrategy)} = {initStrategy} ***");
            DoTest(CreateEstimator(windowSize, k, initStrategy), initStrategy,
                totalElementCount, windowSize, k, _ => (double) random.Next(10_000));
        }
    }

    [Theory]
    [InlineData(20, 5, 0)]
    [InlineData(20, 5, 1)]
    [InlineData(20, 5, 2)]
    [InlineData(20, 5, 3)]
    [InlineData(20, 5, 4)]
    public void MovingSelectorEqualTest(int totalElementCount, int windowSize, int k)
    {
        var random = new Random(42);
        foreach (var initStrategy in new[]
                     {MovingQuantileEstimatorInitStrategy.OrderStatistics, MovingQuantileEstimatorInitStrategy.QuantileApproximation})
        {
            Output.WriteLine($"*** {nameof(MovingQuantileEstimatorInitStrategy)} = {initStrategy} ***");
            DoTest(CreateEstimator(windowSize, k, initStrategy), initStrategy,
                totalElementCount, windowSize, k, _ => (double) random.Next(10_000));
        }
    }

    [Theory]
    [InlineData(300, 5)]
    [InlineData(300, 17)]
    [InlineData(300, 127)]
    [InlineData(10_000, 1023)]
    public void MovingSelectorMedianTest(int totalElementCount, int windowSize)
    {
        DoTest(CreateEstimator(windowSize, Probability.Half), MovingQuantileEstimatorInitStrategy.QuantileApproximation,
            totalElementCount, windowSize, windowSize / 2, _ => 1.0);
    }

    private void DoTest(ISequentialSpecificQuantileEstimator estimator, MovingQuantileEstimatorInitStrategy initStrategy, int totalElementCount,
        int windowSize, int k,
        Func<int, double> generator)
    {
        double[] source = Enumerable.Range(0, totalElementCount).Select(generator).ToArray();

        var outputBuilder = new StringBuilder();
        for (int i = 0; i < source.Length; i++)
        {
            double[] windowElements = source.Take(i + 1).TakeLast(windowSize).ToArray();
            estimator.Add(source[i]);

            if (DiagnosticsMode)
            {
                outputBuilder.AppendLine($"i = {i}");
                outputBuilder.AppendLine(
                    $"Data = [{string.Join(", ", windowElements.Select(x => x.ToString(TestCultureInfo.Instance)))}]");
                if (estimator is PartitioningHeapsMovingQuantileEstimator partitioningHeapsMovingQuantileEstimator)
                    outputBuilder.AppendLine($"Heap = [{partitioningHeapsMovingQuantileEstimator.Dump()}]");
                outputBuilder.AppendLine();
            }

            if (initStrategy == MovingQuantileEstimatorInitStrategy.OrderStatistics && k >= windowElements.Length)
            {
                Assert.Throws<IndexOutOfRangeException>(() => estimator.Quantile());
            }
            else
            {
                double actual = estimator.Quantile();
                Array.Sort(windowElements);
                double expected = initStrategy switch
                {
                    MovingQuantileEstimatorInitStrategy.QuantileApproximation => windowElements[windowElements.Length * k / windowSize],
                    MovingQuantileEstimatorInitStrategy.OrderStatistics => windowElements[Math.Min(windowElements.Length - 1, k)],
                    _ => throw new ArgumentOutOfRangeException()
                };
                Assert.Equal(expected, actual);
            }
        }

        if (DiagnosticsMode)
            Output.WriteLine(outputBuilder.ToString());
    }
}