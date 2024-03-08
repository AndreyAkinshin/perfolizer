namespace Perfolizer.Metrology;

internal static class MeasurementValueExtensions
{
    public static Sample Apply(Sample sample, Func<double, double> applySingle)
    {
        double[] values = new double[sample.Size];
        for (int i = 0; i < sample.Size; i++)
            values[i] = applySingle(sample.Values[i]);
        return sample.IsWeighted
            ? new Sample(values, sample.Weights, sample.Unit)
            : new Sample(values, sample.Unit);
    }

    private static Sample? Apply(this IAbsoluteMeasurementValue self, Sample sample)
    {
        if (self.Unit.GetFlavor() != sample.Unit.GetFlavor())
            return null;
        double shift = self.GetShift(sample);
        return Apply(sample, x => x + shift);
    }

    private static Sample Apply(this IRelativeMeasurementValue self, Sample sample)
    {
        double shift = self.GetRatio();
        return Apply(sample, x => x * shift);
    }

    public static Sample? Apply(this ISpecificMeasurementValue self, Sample sample) => self switch
    {
        IAbsoluteMeasurementValue absoluteMeasurementValue => absoluteMeasurementValue.Apply(sample),
        IRelativeMeasurementValue relativeMeasurementValue => relativeMeasurementValue.Apply(sample),
        _ => throw new NotSupportedException($"Unsupported measurement value type: {self.GetType()}")
    };
}