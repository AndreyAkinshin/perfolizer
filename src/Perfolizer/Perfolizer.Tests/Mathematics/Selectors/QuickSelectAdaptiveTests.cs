using JetBrains.Annotations;
using Perfolizer.Mathematics.Selectors;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Selectors;

[PublicAPI]
public class QuickSelectAdaptiveTests : SelectorTestBase
{
    public QuickSelectAdaptiveTests(ITestOutputHelper output) : base(output)
    {
    }

    private class Adapter : SelectorAdapter
    {
        private readonly double[] values;
        private readonly QuickSelectAdaptive selector = new QuickSelectAdaptive();

        public Adapter(double[] values)
        {
            this.values = values;
        }

        public override double Select(int k) => selector.Select(values, k);
    }

    protected override SelectorAdapter CreateEstimator(double[] values)
    {
        return new Adapter(values);
    }
}