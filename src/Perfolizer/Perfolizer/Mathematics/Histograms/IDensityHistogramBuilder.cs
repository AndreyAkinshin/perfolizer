using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Histograms;

public interface IDensityHistogramBuilder
{
    [Pure]
    DensityHistogram Build(Sample sample, int binCount);
}