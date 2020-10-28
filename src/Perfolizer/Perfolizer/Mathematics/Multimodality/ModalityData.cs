using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Histograms;

namespace Perfolizer.Mathematics.Multimodality
{
    public class ModalityData
    {
        [NotNull] public IReadOnlyList<RangedMode> Modes { get; }

        public DensityHistogram DensityHistogram { get; }

        public int Modality => Modes.Count;

        public ModalityData([NotNull] IReadOnlyList<RangedMode> modes, DensityHistogram densityHistogram)
        {
            Assertion.NotNull(nameof(modes), modes);
            Assertion.NotNull(nameof(densityHistogram), densityHistogram);

            Modes = modes;
            DensityHistogram = densityHistogram;
        }

        [NotNull]
        public string Present()
        {
            var builder = new StringBuilder();
            foreach (var mode in Modes)
                builder.AppendLine($"{mode.Left:0.00} | {mode.Location:0.00} | {mode.Right:0.00}");

            return builder.TrimEnd().ToString();
        }
    }
}