using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Histograms
{
    public interface IDensityHistogramBuilder
    {
        [Pure, NotNull]
        DensityHistogram Build([NotNull] Sample sample, int binCount);
    }
}