using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Histograms;

public interface IHistogramBuilder
{
    [PublicAPI, Pure]
    Histogram Build(IReadOnlyList<double> values);

    [PublicAPI, Pure]
    Histogram Build(IReadOnlyList<double> values, double binSize);
}