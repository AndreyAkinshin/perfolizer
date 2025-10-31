using JetBrains.Annotations;
using Perfolizer.Extensions;
using Pragmastat.Metrology;

namespace Perfolizer.Metrology;

public readonly struct PercentValue(double percentage)
{
    private const string DefaultFormat = "0.###";

    [PublicAPI] public double Percentage { get; } = percentage;
    [PublicAPI] public static PercentValue Of(double percentage) => new(percentage);

    public override string ToString() => Percentage.Format(DefaultFormat) + "%";
    public Measurement ToMeasurement() => new(Percentage, PercentUnit.Instance);

    public MeasurementUnit Unit => PercentUnit.Instance;
    public double GetRatio() => 1.0 + Percentage / 100.0;
}