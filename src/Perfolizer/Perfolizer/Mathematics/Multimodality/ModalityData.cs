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
        [NotNull] public IReadOnlyList<double> Modes { get; }

        [NotNull] public IReadOnlyList<double> CutPoints { get; }

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

        [NotNull]
        public string Present()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < Modality; i++)
            {
                double left = i == 0 ? DensityHistogram.GlobalLower : CutPoints[i - 1];
                double right = i == Modality - 1 ? DensityHistogram.GlobalUpper : CutPoints[i];
                double value = Modes[i];
                builder.AppendLine($"{left:0.00} | {value:0.00} | {right:0.00}");
            }
            
            return builder.TrimEnd().ToString();
        }
    }
}