using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    [UsedImplicitly]
    public class SimpleMovingQuantileEstimatorTests : MovingQuantileEstimatorTestsBase
    {
        public SimpleMovingQuantileEstimatorTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ISequentialSpecificQuantileEstimator CreateEstimator(int windowSize, int k, MovingQuantileEstimatorInitStrategy initStrategy)
        {
            return new SimpleMovingQuantileEstimator(windowSize, k, initStrategy);
        }

        protected override ISequentialSpecificQuantileEstimator CreateEstimator(int windowSize, Probability p)
        {
            return new SimpleMovingQuantileEstimator(windowSize, p);
        }
    }
}