using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.ScaleEstimators;
using Perfolizer.Tests.Common;

namespace Perfolizer.Tests.Mathematics.ScaleEstimators;

public class ScaleEstimatorTestsBase
{
    private readonly ITestOutputHelper output;
    private readonly AbsoluteEqualityComparer comparer = new(1e-5);

    protected ScaleEstimatorTestsBase(ITestOutputHelper output)
    {
        this.output = output;
    }

    public static IEnumerable<object[]> CommonConsistencySampleSizes => new[] { new object[] { 3 }, [5], [10], [50] };

    protected void CheckConsistency(int sampleSize, IScaleEstimator scaleEstimator)
    {
        var distribution = NormalDistribution.Standard;
        var randomGenerator = distribution.Random(new Random(1729));
        var estimations = new List<double>();
        for (int i = 0; i < 10_000; i++)
        {
            var sample = new Sample(randomGenerator.Next(sampleSize));
            estimations.Add(scaleEstimator.Scale(sample));
        }
        double bias = Math.Abs(estimations.Average() - 1);
        output.WriteLine("Bias = " + bias.ToStringInvariant());
        Assert.True(bias < 0.02); // TODO: improve, make it 0.005
    }

    protected void Check(IScaleEstimator estimator, double expected, params double[] values)
    {
        var sample = new Sample(values);
        double actual = estimator.Scale(sample);
        Assert.Equal(expected, actual, comparer);
    }
}