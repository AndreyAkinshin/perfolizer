using System.Diagnostics.CodeAnalysis;
using Perfolizer.Horology;

namespace Perfolizer.Mathematics.Thresholds;

public class AbsoluteTimeThreshold : AbsoluteThreshold, IEquatable<AbsoluteTimeThreshold?>
{
    private readonly TimeInterval timeInterval;

    public AbsoluteTimeThreshold(TimeInterval timeInterval) : base(timeInterval.Nanoseconds) => this.timeInterval = timeInterval;

    public override string ToString() => timeInterval.ToString("0.##");

    public bool Equals([NotNullWhen(true)] AbsoluteTimeThreshold? other) => other != null && timeInterval.Equals(other.timeInterval);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is AbsoluteTimeThreshold other && Equals(other);

    public override int GetHashCode() => timeInterval.GetHashCode();
}