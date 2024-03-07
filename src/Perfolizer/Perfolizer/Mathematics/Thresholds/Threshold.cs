using Perfolizer.Common;

namespace Perfolizer.Mathematics.Thresholds;

public abstract class Threshold
{
    public abstract Sample Apply(Sample sample);

    protected Sample Apply(Sample sample, Func<double, double> applySingle)
    {
        double[] values = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
            values[i] = applySingle(sample.Values[i]);
        return sample.IsWeighted ? new Sample(values, sample.Weights) : new Sample(values);
    }
}