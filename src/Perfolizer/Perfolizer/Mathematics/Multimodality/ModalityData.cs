using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Histograms;

namespace Perfolizer.Mathematics.Multimodality
{
    public class ModalityData
    {
        [NotNull]
        public IReadOnlyList<double> Modes { get; }

        [NotNull]
        public IReadOnlyList<double> CutPoints { get; }

        public DensityHistogram DensityHistogram { get; }

        public int Modality => Modes.Count;

        public ModalityData([NotNull] IReadOnlyList<double> modes, [NotNull] IReadOnlyList<double> cutPoints,
            DensityHistogram densityHistogram)
        {
            Assertion.NotNull(nameof(modes), modes);
            Assertion.NotNull(nameof(cutPoints), cutPoints);
            if (modes.Count != cutPoints.Count + 1)
                throw new ArgumentOutOfRangeException(
                    $"{nameof(modes)}.Count should be equal to {nameof(cutPoints)}.Count + 1 " +
                    $"({nameof(modes)}.Count = {modes.Count}, {nameof(cutPoints)}.Count = {cutPoints.Count})");
            Assertion.NotNull(nameof(densityHistogram), densityHistogram);

            Modes = modes;
            CutPoints = cutPoints;
            DensityHistogram = densityHistogram;
        }
    }
}