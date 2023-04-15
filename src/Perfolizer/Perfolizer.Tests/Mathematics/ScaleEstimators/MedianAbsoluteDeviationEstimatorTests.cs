using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Tests.Mathematics.ScaleEstimators;

public class MedianAbsoluteDeviationEstimatorTests : ScaleEstimatorTestsBase
{
    public MedianAbsoluteDeviationEstimatorTests(ITestOutputHelper output) : base(output)
    {
    }
        
    [Theory]
    [MemberData(nameof(CommonConsistencySampleSizes))]
    public void SimpleMadConsistencyTest(int sampleSize)
        => CheckConsistency(sampleSize, MedianAbsoluteDeviationEstimator.Simple);

    [Theory]
    [MemberData(nameof(CommonConsistencySampleSizes))]
    public void HdMadConsistencyTest(int sampleSize)
        => CheckConsistency(sampleSize, MedianAbsoluteDeviationEstimator.HarrellDavis);
}