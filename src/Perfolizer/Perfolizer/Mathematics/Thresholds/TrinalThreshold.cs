using Perfolizer.Common;
using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Mathematics.Thresholds;

/// <summary>
/// https://aakinshin.net/posts/trinal-thresholds/
/// </summary>
public class TrinalThreshold : Threshold
{
    public double? Shift { get; }
    public double? Ratio { get; }
    public double? EffectSize { get; }

    public TrinalThreshold(double? shift, double? ratio, double? effectSize)
    {
        if (shift == null && ratio == null && effectSize == null)
            throw new ArgumentException("At least one of the shift, ratio, or effectSize must be non-null");

        Shift = shift;
        Ratio = ratio;
        EffectSize = effectSize;
    }

    public override Sample Apply(Sample sample)
    {
        double scale = ShamosEstimator.Instance.Scale(sample);

        double[] values = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
        {
            double value = sample.Values[i];
            if (Shift != null)
                value = Max(value, sample.Values[i] + Shift.Value);
            if (Ratio != null)
                value = Max(value, sample.Values[i] * Ratio.Value);
            if (EffectSize != null)
                value = Max(value, sample.Values[i] + EffectSize.Value * scale);

            values[i] = value;
        }
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }
}