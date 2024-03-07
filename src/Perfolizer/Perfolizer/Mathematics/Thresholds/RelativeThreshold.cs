using System.Diagnostics.CodeAnalysis;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Thresholds;

public class RelativeThreshold(double ratio) : Threshold, IEquatable<RelativeThreshold?>
{
    public static readonly Threshold Zero = new RelativeThreshold(0);
    public static readonly Threshold Unit = new RelativeThreshold(1);
    public static RelativeThreshold Of(double ratio) => new(ratio);

    private readonly double ratio = ratio;

    public override Sample Apply(Sample sample) => Apply(sample, x => x * ratio);
    public override string ToString() => (ratio * 100).ToStringInvariant("N4") + "%";

    public bool Equals([NotNullWhen(true)] RelativeThreshold? other) => other != null && ratio.Equals(other.ratio);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is RelativeThreshold other && Equals(other);
    public override int GetHashCode() => ratio.GetHashCode();
}