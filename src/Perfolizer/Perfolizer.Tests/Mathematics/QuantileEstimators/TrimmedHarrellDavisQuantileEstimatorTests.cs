using System.Collections.Generic;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public class TrimmedHarrellDavisQuantileEstimatorTests
    {
        private static readonly IEqualityComparer<double> Comparer = new AbsoluteEqualityComparer(1e-7);
        
        [Theory]
        [InlineData(10, 0, 0.3, 0.7, 1)]
        [InlineData(0, 10, 0.3, 0, 0.3)]
        [InlineData(1, 1, 0.5, 0.25, 0.75)]
        [InlineData(3, 3, 0.3, 0.35, 0.65)]
        [InlineData(7, 3, 0.3, 0.5797299, 0.8797299)]
        [InlineData(7, 13, 0.3, 0.1947799, 0.4947799)]
        public void BetaHdiTest(double a, double b, double width, double l, double r)
        {
            var hdi = TrimmedHarrellDavisQuantileEstimator.GetBetaHdi(a, b, width);
            Assert.Equal(l, hdi.L, Comparer);
            Assert.Equal(r, hdi.R, Comparer);
        }
    }
}