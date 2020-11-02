using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Histograms;
using Perfolizer.Mathematics.OutlierDetection;

namespace Perfolizer.Mathematics.Multimodality
{
    public class ModalityData
    {
        [NotNull, PublicAPI]
        public IReadOnlyList<RangedMode> Modes { get; }

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

        private class Bunch
        {
            private double min, max;
            private int count;

            public Bunch()
            {
                Clear();
            }

            public void Clear()
            {
                min = double.MaxValue;
                max = double.MinValue;
                count = 0;
            }

            public void Add(double value)
            {
                min = Math.Min(min, value);
                max = Math.Max(max, value);
                count++;
            }

            public bool Any() => count > 0;

            public void Present([NotNull] StringBuilder builder, char open, string multiSeparator, char close, string format,
                IFormatProvider formatProvider)
            {
                switch (count)
                {
                    case 0:
                        break;
                    case 1:
                        builder.Append(open);
                        builder.Append(min.ToString(format, formatProvider));
                        builder.Append(close);
                        break;
                    case 2:
                        builder.Append(open);
                        builder.Append(min.ToString(format, formatProvider));
                        builder.Append(", ");
                        builder.Append(max.ToString(format, formatProvider));
                        builder.Append(close);
                        break;
                    default:
                    {
                        builder.Append(open);
                        builder.Append(min.ToString(format, formatProvider));
                        builder.Append(multiSeparator);
                        builder.Append(max.ToString(format, formatProvider));
                        builder.Append(close);
                        break;
                    }
                }
            }
        }

        [NotNull, Pure]
        public string PresentSummary(IOutlierDetectorFactory outlierDetectorFactory, string format, IFormatProvider formatProvider)
        {
            var builder = new StringBuilder();

            var bunch = new Bunch();
            bool isFirst = true;

            void PresentBunch(char open, string multiSeparator, char close)
            {
                if (bunch.Any())
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        builder.Append(" + ");
                    bunch.Present(builder, open, multiSeparator, close, format, formatProvider);
                }
            }

            foreach (var mode in Modes)
            {
                var outlierDetector = outlierDetectorFactory.Create(mode.Values);
                int index = 0;
                
                // *Lower outliers*
                while (index < mode.Values.Count && outlierDetector.IsLowerOutlier(mode.Values[index]))
                    bunch.Add(mode.Values[index++]);
                PresentBunch('{', "..", '}');
                bunch.Clear();
                
                // *Central values*
                while (index < mode.Values.Count && !outlierDetector.IsOutlier(mode.Values[index]))
                    bunch.Add(mode.Values[index++]);
                PresentBunch('[', "; ", ']');
                bunch.Clear();
                
                // *Upper outliers*
                while (index < mode.Values.Count && outlierDetector.IsUpperOutlier(mode.Values[index]))
                    bunch.Add(mode.Values[index++]);
                // Propagate bunch to the lower outliers of the next mode
            }
            PresentBunch('{', "..", '}'); // Upper outliers of the last mode
            return builder.ToString();
        }
    }
}