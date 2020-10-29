using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Histograms
{
    public interface IDensityHistogramBuilder
    {
        [Pure, NotNull]
        DensityHistogram Build([NotNull] IReadOnlyList<double> values, int binCount);
        
        [Pure, NotNull]
        DensityHistogram Build([NotNull] IReadOnlyList<double> values, IReadOnlyList<double> weights, int binCount);
    }
}