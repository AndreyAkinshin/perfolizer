using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Histograms
{
    public static class DensityHistogramBuilderExtensions
    {
        public static DensityHistogram Build(this IDensityHistogramBuilder builder, IReadOnlyList<double> values, int binCount)
        {
            Assertion.NotNull(nameof(builder), builder);

            return builder.Build(new Sample(values), binCount);
        }

        public static DensityHistogram Build(this IDensityHistogramBuilder builder, IReadOnlyList<double> values,
            IReadOnlyList<double> weights, int binCount)
        {
            Assertion.NotNull(nameof(builder), builder);

            return builder.Build(new Sample(values, weights), binCount);
        }
    }
}