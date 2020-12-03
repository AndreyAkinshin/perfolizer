using System.Globalization;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;
using Perfolizer.Tests.Common;
using Xunit;

namespace Perfolizer.Tests.Mathematics.RangeEstimator
{
    public class RangeTests
    {
        [Theory]
        [InlineData(1, 2, null, null, "[1.00;2.00]")]
        [InlineData(1.2345, 2.2345, null, null, "[1.23;2.23]")]
        [InlineData(1.2345, 2.2345, "ru-ru", null, "[1,23;2,23]")]
        [InlineData(1.2345, 2.2345, null, "N4", "[1.2345;2.2345]")]
        [InlineData(1.2345, 2.2345, "ru-ru", "N4", "[1,2345;2,2345]")]
        public void RangeToString(double left, double right, [CanBeNull] string cultureName, [CanBeNull] string format, string expected)
        {
            var range = Range.Of(left, right);
            var cultureInfo = cultureName == null ? null : new CultureInfo(cultureName);

            Assert.Equal(expected, range.ToString(cultureInfo, format));

            if (cultureInfo == null)
                Assert.Equal(expected, range.ToString(format));

            if (cultureInfo == null && format == null)
                Assert.Equal(expected, range.ToString());

            string leftString = range.Left.ToString(format ?? "N2", cultureInfo ?? TestCultureInfo.Instance);
            string rightString = range.Right.ToString(format ?? "N2", cultureInfo ?? TestCultureInfo.Instance);
            string rangeString = range.ToString(cultureInfo, format);
            Assert.Contains(leftString, rangeString);
            Assert.Contains(rightString, rangeString);
        }

        [Theory]
        [InlineData(1, 2, 1, 2, true)]
        [InlineData(1, 1, 1, 1, true)]
        [InlineData(2, 3, 4, 5, false)]
        [InlineData(4, 5, 2, 3, false)]
        [InlineData(1, 2, 1, 3, true)]
        [InlineData(2, 3, 1, 3, true)]
        [InlineData(1, 3, 2, 4, false)]
        [InlineData(2, 4, 1, 3, false)]
        public void RangeIsInside(double innerLeft, double innerRight, double outerLeft, double outerRight, bool expected)
        {
            var innerRange = Range.Of(innerLeft, innerRight);
            var outerRange = Range.Of(outerLeft, outerRight);
            Assert.Equal(expected, innerRange.IsInside(outerRange));
        }
    }
}