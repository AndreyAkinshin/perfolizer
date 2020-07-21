using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Histograms;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.Multimodality
{
    public class ModalityDetector : IModalityDetector
    {
        [NotNull] public static readonly ModalityDetector Instance = new ModalityDetector();
        
        public ModalityData DetectModes(IReadOnlyList<double> values, [CanBeNull] IReadOnlyList<double> weights = null)
        {
            Assertion.NotNullOrEmpty(nameof(values), values);
            Assertion.MoreThan($"{nameof(values)}.Count", values.Count, 0);
            if (values.Max() - values.Min() < 1e-9)
                throw new ArgumentException($"{nameof(values)} should contain at least two different elements", nameof(values));

            var histogram = QuantileRespectfulDensityHistogramBuilder.Instance.Build(values,
                QuantileRespectfulDensityHistogramBuilder.DefaultBinCount, weights, HarrellDavisQuantileEstimator.Instance);
            var bins = histogram.Bins;
            int binCount = bins.Count;
            double binSquare = 1.0 / bins.Count;
            var binHeights = bins.Select(bin => bin.Height).ToList();

            var peaks = new List<int>();
            for (int i = 1; i < binCount - 1; i++)
                if (binHeights[i] > binHeights[i - 1] && binHeights[i] >= binHeights[i + 1])
                    peaks.Add(i);

            switch (peaks.Count)
            {
                case 0:
                    return new ModalityData(new[] {bins[binHeights.WhichMax()].Middle}, Array.Empty<double>(), histogram);
                case 1:
                    return new ModalityData(new[] {bins[peaks.First()].Middle}, Array.Empty<double>(), histogram);
                default:
                {
                    var modes = new List<double>();
                    var cutPoints = new List<double>();

                    bool CheckForSplit(int peak1, int peak2)
                    {
                        int left = peak1, right = peak2;
                        double height = Math.Min(binHeights[peak1], binHeights[peak2]);
                        while (left < right && binHeights[left] > height)
                            left++;
                        while (left < right && binHeights[right] > height)
                            right--;

                        double width = bins[right].Upper - bins[left].Lower;
                        double totalSquare = width * height;
                        double totalBinSquare = (right - left + 1) * binSquare;
                        if (totalBinSquare < totalSquare - totalBinSquare)
                        {
                            modes.Add(bins[peak1].Middle);
                            cutPoints.Add(bins[binHeights.WhichMin(peak1, peak2 - peak1)].Middle);
                            return true;
                        }

                        return false;
                    }

                    var previousPeaks = new List<int> {peaks[0]};
                    for (int i = 1; i < peaks.Count; i++)
                    {
                        int currentPeak = peaks[i];
                        if (binHeights[previousPeaks.Last()] > binHeights[currentPeak])
                        {
                            if (CheckForSplit(previousPeaks.Last(), currentPeak))
                            {
                                previousPeaks.Clear();
                                previousPeaks.Add(currentPeak);
                            }
                        }
                        else
                        {
                            while (previousPeaks.Any() && binHeights[previousPeaks.Last()] < binHeights[currentPeak])
                            {
                                if (CheckForSplit(previousPeaks.Last(), currentPeak))
                                {
                                    previousPeaks.Clear();
                                    break;
                                }

                                previousPeaks.RemoveAt(previousPeaks.Count - 1);
                            }

                            previousPeaks.Add(currentPeak);
                        }
                    }

                    modes.Add(bins[previousPeaks.First()].Middle);

                    return new ModalityData(modes, cutPoints, histogram);
                }
            }
        }
    }
}