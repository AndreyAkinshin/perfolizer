using JetBrains.Annotations;
using Perfolizer.Mathematics.Selectors;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Selectors
{
    [PublicAPI]
    public class SimpleSelectorTests: SelectorTestBase
    {
        public SimpleSelectorTests(ITestOutputHelper output) : base(output)
        {
        }

        private class Adapter : SelectorAdapter
        {
            private readonly SimpleSelector selector;

            public Adapter(double[] values)
            {
                selector = new SimpleSelector(values);
            }

            public override double Select(int k) => selector.Select(k);
        }

        protected override SelectorAdapter CreateEstimator(double[] values) => new Adapter(values);
    }
}