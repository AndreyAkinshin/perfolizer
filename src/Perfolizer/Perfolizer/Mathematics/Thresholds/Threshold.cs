using Perfolizer.Common;

namespace Perfolizer.Mathematics.Thresholds;

public abstract class Threshold
{
    public abstract double Apply(double x);

    public Sample Apply(Sample sample)
    {
        double[] values = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
            values[i] = Apply(sample.Values[i]);
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }
}