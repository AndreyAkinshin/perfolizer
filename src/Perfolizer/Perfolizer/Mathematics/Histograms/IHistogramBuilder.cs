using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Histograms
{
    public interface IHistogramBuilder
    {
        [PublicAPI, Pure, NotNull]
        Histogram Build([NotNull] IReadOnlyList<double> values);

        [PublicAPI, Pure, NotNull]
        Histogram Build([NotNull] IReadOnlyList<double> values, double binSize);
    }
}