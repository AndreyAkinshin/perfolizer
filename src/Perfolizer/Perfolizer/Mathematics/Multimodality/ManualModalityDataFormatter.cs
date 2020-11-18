using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.OutlierDetection;

namespace Perfolizer.Mathematics.Multimodality
{
    public class ManualModalityDataFormatter : IModalityDataFormatter
    {
        [NotNull]
        public static IModalityDataFormatter Default() => new ManualModalityDataFormatter();

        [NotNull]
        public static IModalityDataFormatter Compact() => new ManualModalityDataFormatter
        {
            PresentCount = false,
            PresentModeLocations = false,
            PresentOutliers = false,
            CompactMiddleModes = true
        };

        [NotNull]
        public static IModalityDataFormatter Full() => new ManualModalityDataFormatter
        {
            PresentCount = true,
            PresentModeLocations = true,
            PresentOutliers = true,
            CompactMiddleModes = false
        };

        [CanBeNull] public IOutlierDetectorFactory OutlierDetectorFactory { get; set; }
        public bool PresentCount { get; set; } = true;
        public bool PresentModeLocations { get; set; } = true;
        public bool PresentOutliers { get; set; } = true;
        public bool CompactMiddleModes { get; set; } = true;
        public string GroupSeparator { get; set; } = " + ";

        public string Format(ModalityData data, string numberFormat = null, IFormatProvider numberFormatProvider = null)
        {
            Assertion.NotNull(nameof(data), data);

            var outlierDetectorFactory = OutlierDetectorFactory ?? SimpleOutlierDetectorFactory.DoubleMad;
            numberFormat ??= "N2";
            numberFormatProvider ??= DefaultCultureInfo.Instance;

            bool compactMode = CompactMiddleModes && data.Modality > 2;
            var modes = compactMode
                ? new[] {data.Modes.First(), data.Modes.Last()}
                : data.Modes;
            var builder = new StringBuilder();
            var bunch = new Bunch();
            bool isFirst = true;

            void AddBunch(char open, string multiSeparator, char close)
            {
                if (bunch.Any())
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        builder.Append(GroupSeparator);
                    bunch.Present(builder, open, multiSeparator, close, PresentCount, numberFormat, numberFormatProvider);
                }
            }

            void AddMode() => AddBunch('[', "; ", ']');
            void AddOutliers() => AddBunch('{', "..", '}');

            void AddMiddleNodesIfNeeded(int index)
            {
                if (index == 0 && compactMode)
                {
                    int extraModes = data.Modality - 2;
                    if (isFirst)
                        isFirst = false;
                    else
                        builder.Append(GroupSeparator);
                    builder.Append('<');
                    builder.Append(extraModes);
                    builder.Append(' ');
                    builder.Append(extraModes > 1 ? "modes" : "mode");
                    builder.Append('>');
                }
            }


            if (PresentOutliers)
            {
                for (int i = 0; i < modes.Count; i++)
                {
                    var mode = modes[i];
                    var outlierDetector = outlierDetectorFactory.Create(mode.Values);
                    int index = 0;

                    // *Lower outliers*
                    while (index < mode.Values.Count && outlierDetector.IsLowerOutlier(mode.Values[index]))
                        bunch.Add(mode.Values[index++]);
                    if (!(compactMode && i != 0))
                        AddOutliers();
                    bunch.Clear();

                    // *Central values*
                    while (index < mode.Values.Count && !outlierDetector.IsOutlier(mode.Values[index]))
                        bunch.Add(mode.Values[index++]);
                    if (PresentModeLocations)
                        bunch.Mode = mode.Location;
                    AddMode();
                    bunch.Clear();

                    // *Upper outliers*
                    while (index < mode.Values.Count && outlierDetector.IsUpperOutlier(mode.Values[index]))
                        bunch.Add(mode.Values[index++]);
                    // Propagate bunch to the lower outliers of the next mode

                    AddMiddleNodesIfNeeded(i);
                }

                AddOutliers(); // Upper outliers of the last mode
            }
            else
            {
                for (int i = 0; i < modes.Count; i++)
                {
                    var mode = modes[i];
                    bunch.Min = mode.Min();
                    bunch.Max = mode.Max();
                    if (PresentModeLocations)
                        bunch.Mode = mode.Location;
                    bunch.Count = mode.Values.Count;
                    AddBunch('[', "; ", ']');
                    AddMiddleNodesIfNeeded(i);
                }
            }

            return builder.ToString();
        }

        private class Bunch
        {
            public double Min { get; set; }
            public double Max { get; set; }
            public double? Mode { get; set; }
            public int Count { get; set; }

            public Bunch()
            {
                Clear();
            }

            public void Clear()
            {
                Min = double.MaxValue;
                Max = double.MinValue;
                Mode = null;
                Count = 0;
            }

            public void Add(double value)
            {
                Min = Math.Min(Min, value);
                Max = Math.Max(Max, value);
                Count++;
            }

            public bool Any() => Count > 0;

            public void Present([NotNull] StringBuilder builder, char open, string multiSeparator, char close, bool presentCount,
                [NotNull] string format, [NotNull] IFormatProvider formatProvider)
            {
                switch (Count)
                {
                    case 0:
                        break;
                    case 1:
                        builder.Append(open);
                        builder.Append(Min.ToString(format, formatProvider));
                        builder.Append(close);
                        break;
                    case 2:
                        builder.Append(open);
                        builder.Append(Min.ToString(format, formatProvider));
                        builder.Append(", ");
                        builder.Append(Max.ToString(format, formatProvider));
                        builder.Append(close);
                        break;
                    default:
                    {
                        builder.Append(open);
                        if (Mode != null)
                        {
                            builder.Append(Min.ToString(format, formatProvider));
                            builder.Append(" | ");
                            builder.Append(Mode?.ToString(format, formatProvider));
                            builder.Append(" | ");
                            builder.Append(Max.ToString(format, formatProvider));
                        }
                        else
                        {
                            builder.Append(Min.ToString(format, formatProvider));
                            builder.Append(multiSeparator);
                            builder.Append(Max.ToString(format, formatProvider));
                        }

                        builder.Append(close);
                        break;
                    }
                }

                if (Count > 2 && presentCount)
                {
                    builder.Append("_");
                    builder.Append(Count);
                }
            }
        }
    }
}