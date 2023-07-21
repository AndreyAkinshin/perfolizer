using System.Diagnostics.CodeAnalysis;

namespace Perfolizer.Mathematics.Thresholds;

public class RelativeThreshold : Threshold, IEquatable<RelativeThreshold?>
{
    public static readonly Threshold Zero = new RelativeThreshold(0);
    public static readonly Threshold Unit = new RelativeThreshold(1);

    private readonly double ratio;

    public RelativeThreshold(double ratio) => this.ratio = ratio;

    public override double Apply(double x) => x * ratio;
    public override string ToString() => (ratio * 100).ToString("N4") + "%";

    public bool Equals([NotNullWhen(true)] RelativeThreshold? other) => other != null && ratio.Equals(other.ratio);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is RelativeThreshold other && Equals(other);
    public override int GetHashCode() => ratio.GetHashCode();
}