using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Histograms;

namespace Perfolizer.Mathematics.Multimodality;

public class ModalityData
{
    [PublicAPI] public IReadOnlyList<RangedMode> Modes { get; }

    public DensityHistogram DensityHistogram { get; }

    public int Modality => Modes.Count;

    public ModalityData(IReadOnlyList<RangedMode> modes, DensityHistogram densityHistogram)
    {
        Assertion.NotNull(nameof(modes), modes);
        Assertion.NotNull(nameof(densityHistogram), densityHistogram);

        Modes = modes;
        DensityHistogram = densityHistogram;
    }
}