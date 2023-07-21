using Perfolizer.Common;

namespace Perfolizer.Mathematics.Thresholds;

public class AbsoluteThreshold : Threshold
{
    public static readonly AbsoluteThreshold Zero = new(0);

    private readonly double value;

    public AbsoluteThreshold(double value) => this.value = value;

    public override double Apply(double x) => x + value;
    public override string ToString() => value.ToStringInvariant("0.##");
}