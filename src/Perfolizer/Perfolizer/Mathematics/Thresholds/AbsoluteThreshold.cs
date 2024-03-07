using Perfolizer.Common;

namespace Perfolizer.Mathematics.Thresholds;

public class AbsoluteThreshold(double shift) : Threshold
{
    public static readonly AbsoluteThreshold Zero = new(0);
    public static AbsoluteThreshold Of(double shift) => new(shift);

    public override string ToString() => shift.ToStringInvariant("0.##");
    public override Sample Apply(Sample sample) => Apply(sample, x => x + shift);
}