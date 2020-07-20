using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Histograms
{
    public class DensityHistogram
    {
        [NotNull]
        public IReadOnlyList<DensityHistogramBin> Bins { get; }

        public DensityHistogram([NotNull] IReadOnlyList<DensityHistogramBin> bins)
        {
            Bins = bins;
        }
    }
}