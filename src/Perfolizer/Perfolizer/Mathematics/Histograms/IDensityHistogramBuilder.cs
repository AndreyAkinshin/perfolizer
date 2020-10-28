using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Histograms
{
    public interface IDensityHistogramBuilder
    {
        [Pure, NotNull]
        DensityHistogram Build([NotNull] IReadOnlyList<double> values);
        
        [Pure, NotNull]
        DensityHistogram Build([NotNull] IReadOnlyList<double> values, IReadOnlyList<double> weights);
    }
}