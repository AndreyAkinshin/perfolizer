using System.Collections.Generic;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Histograms;

namespace Perfolizer.Mathematics.Multimodality
{
    internal class LowlandModalityDiagnosticsData : ModalityData
    {
        public IReadOnlyList<DiagnosticsBin> Bins { get; }

        public LowlandModalityDiagnosticsData(
            IReadOnlyList<RangedMode> modes,
            DensityHistogram densityHistogram,
            IReadOnlyList<DiagnosticsBin> bins)
            : base(modes, densityHistogram)
        {
            Bins = bins;
        }

        public class DiagnosticsBin
        {
            public DensityHistogramBin HistogramBin { get; }
            public double WaterLevel { get; set; }
            public bool IsMode { get; set; }
            public bool IsLowland { get; set; }
            public bool IsPeak { get; set; }

            public DiagnosticsBin(DensityHistogramBin histogramBin)
            {
                HistogramBin = histogramBin;
                WaterLevel = histogramBin.Height;
                IsMode = false;
                IsLowland = false;
            }
        }

        public void DumpAsCsv(StreamWriter writer, CultureInfo? cultureInfo = null)
        {
            cultureInfo ??= DefaultCultureInfo.Instance;
            writer.WriteLine("index,left,right,height,water,isMode,isPeak,isLowland");
            for (int i = 0; i < Bins.Count; i++)
            {
                var bin = Bins[i];
                writer.Write(i);
                writer.Write(",");
                writer.Write(bin.HistogramBin.Lower.ToString("N7", cultureInfo));
                writer.Write(",");
                writer.Write(bin.HistogramBin.Upper.ToString("N7", cultureInfo));
                writer.Write(",");
                writer.Write(bin.HistogramBin.Height.ToString("N5", cultureInfo));
                writer.Write(",");
                writer.Write(bin.WaterLevel.ToString("N5", cultureInfo));
                writer.Write(",");
                writer.Write(bin.IsMode.ToString().ToLowerInvariant());
                writer.Write(",");
                writer.Write(bin.IsPeak.ToString().ToLowerInvariant());
                writer.Write(",");
                writer.Write(bin.IsLowland.ToString().ToLowerInvariant());
                writer.WriteLine();
            }

            writer.WriteLine();
        }
    }
}