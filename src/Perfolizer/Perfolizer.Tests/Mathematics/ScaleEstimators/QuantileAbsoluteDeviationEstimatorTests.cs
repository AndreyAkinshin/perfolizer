using Perfolizer.Collections;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Tests.Mathematics.ScaleEstimators;

public class QuantileAbsoluteDeviationEstimatorTests : ScaleEstimatorTestsBase
{
    public QuantileAbsoluteDeviationEstimatorTests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [MemberData(nameof(CommonConsistencySampleSizes))]
    public void StandardQadConsistencyTest(int sampleSize)
        => CheckConsistency(sampleSize, QuantileAbsoluteDeviationEstimator.Standard);

    [Theory]
    [MemberData(nameof(CommonConsistencySampleSizes))]
    public void OptimalQadConsistencyTest(int sampleSize)
        => CheckConsistency(sampleSize, QuantileAbsoluteDeviationEstimator.Optimal);

    [Fact]
    public void MadQadConsistencyTest()
    {
        var madEstimator = MedianAbsoluteDeviationEstimator.Invariant;
        var qadEstimator = QuantileAbsoluteDeviationEstimator.Invariant(0.5);

        var sample = NormalDistribution.Standard.Random(1729).Next(42).ToSample();
        Assert.Equal(madEstimator.Mad(sample), qadEstimator.Qad(sample));
    }

    [Fact]
    public void QadPropertyTest()
    {
        Assert.Equal(Probability.Of(0.3), QuantileAbsoluteDeviationEstimator.Invariant(0.3).P);
        Assert.Equal(Probability.Of(0.7), QuantileAbsoluteDeviationEstimator.Invariant(0.7).P);
        Assert.Equal(Probability.Of(0.682689492137086), QuantileAbsoluteDeviationEstimator.Standard.P);
        Assert.Equal(Probability.Of(0.861678977787423), QuantileAbsoluteDeviationEstimator.Optimal.P);

        Assert.Equal(SimpleQuantileEstimator.Instance, QuantileAbsoluteDeviationEstimator.Invariant(0.5).QuantileEstimator);
        Assert.Equal(SimpleQuantileEstimator.Instance, QuantileAbsoluteDeviationEstimator.Standard.QuantileEstimator);
        Assert.Equal(SimpleQuantileEstimator.Instance, QuantileAbsoluteDeviationEstimator.Optimal.QuantileEstimator);
    }
}