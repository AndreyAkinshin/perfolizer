using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Tests.Mathematics.ScaleEstimators;

public class ShamosEstimatorTests(ITestOutputHelper output) : ScaleEstimatorTestsBase(output)
{
    [Fact]
    public void ShamosTest01() => Check(ShamosEstimator.Instance, 0.807087317351071, 1, 2, 3);

    [Fact]
    public void ShamosTest02() => Check(ShamosEstimator.Instance, 6.18823369368787, 1, 2, 4, 8, 16);

    [Theory]
    [MemberData(nameof(CommonConsistencySampleSizes))]
    public void ShamosConsistencyTest(int sampleSize) => CheckConsistency(sampleSize, ShamosEstimator.Instance);
}