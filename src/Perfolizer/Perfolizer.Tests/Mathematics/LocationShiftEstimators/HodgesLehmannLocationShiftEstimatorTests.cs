using Perfolizer.Collections;
using Perfolizer.Mathematics.LocationShiftEstimators;
using Perfolizer.Tests.Common;
using Xunit;

namespace Perfolizer.Tests.Mathematics.LocationShiftEstimators
{
    public class HodgesLehmannLocationShiftEstimatorTests
    {
        [Fact]
        public void HodgesLehmannLocationShiftEstimatorTest01()
        {
            Check(
                new[] { 0.298366502872861, 2.30272056972301, -1.07018041144338, 0.967248885283515, -0.849008187096325 },
                new[] { 9.98634587887872, 8.78621971483415, 9.35864761227285, 9.80372149505987, 10.2586337161638 },
                9.50535499218701
            );
        }

        private void Check(double[] a, double[] b, double expectedLocationShift)
        {
            double actualLocationShift = HodgesLehmannLocationShiftEstimator.Instance.LocationShift(a.ToSample(), b.ToSample());
            Assert.Equal(expectedLocationShift, actualLocationShift, AbsoluteEqualityComparer.E9);
        }
    }
}