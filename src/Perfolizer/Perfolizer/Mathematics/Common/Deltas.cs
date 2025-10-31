using JetBrains.Annotations;
using Pragmastat.Metrology;

namespace Perfolizer.Mathematics.Common;

public class Deltas(Measurement shift, Measurement ratio, Measurement disparity)
{
    [PublicAPI]
    public double Shift { get; } = shift;

    [PublicAPI]
    public double Ratio { get; } = ratio;

    [PublicAPI]
    public double Disparity { get; } = disparity;

    [PublicAPI]
    public bool IsBelow(Deltas other) => Shift < other.Shift && Ratio < other.Ratio && Disparity < other.Disparity;

    [PublicAPI]
    public bool IsAbove(Deltas other) => Shift > other.Shift && Ratio > other.Ratio && Disparity > other.Disparity;
}