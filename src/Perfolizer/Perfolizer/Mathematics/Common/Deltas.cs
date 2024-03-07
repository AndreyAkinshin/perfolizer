using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Common;

public class Deltas(double shift, double ratio, double effectSize)
{
    [PublicAPI]
    public double Shift { get; } = shift;

    [PublicAPI]
    public double Ratio { get; } = ratio;

    [PublicAPI]
    public double EffectSize { get; } = effectSize;

    [PublicAPI]
    public bool IsBelow(Deltas other) => Shift < other.Shift && Ratio < other.Ratio && EffectSize < other.EffectSize;

    [PublicAPI]
    public bool IsAbove(Deltas other) => Shift > other.Shift && Ratio > other.Ratio && EffectSize > other.EffectSize;
}