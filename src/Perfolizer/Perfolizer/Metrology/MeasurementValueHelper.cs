namespace Perfolizer.Metrology;

internal static class MeasurementValueHelper
{
    public static Sample Apply(Sample sample, Func<double, double> applySingle)
    {
        double[] values = new double[sample.Size];
        for (int i = 0; i < sample.Size; i++)
            values[i] = applySingle(sample.Values[i]);
        return sample.IsWeighted
            ? new Sample(values, sample.Weights, sample.MeasurementUnit)
            : new Sample(values, sample.MeasurementUnit);
    }
}