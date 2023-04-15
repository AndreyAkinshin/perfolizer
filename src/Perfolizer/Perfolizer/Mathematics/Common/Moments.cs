using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common;

public readonly struct Moments
{
    public double Mean { get; }
    public double Variance { get; }
    public double Skewness { get; }
    public double Kurtosis { get; }

    public double StandardDeviation { get; }

    public Moments(double mean, double variance, double skewness, double kurtosis) : this()
    {
        Mean = mean;
        Variance = variance;
        Skewness = skewness;
        Kurtosis = kurtosis;
        StandardDeviation = Math.Sqrt(Variance);
    }

    public static Moments Create(IReadOnlyList<double> values)
    {
        Assertion.NotNull(nameof(values), values);
        int n = values.Count;
        double mean = values.Average();
        double variance = n == 1 ? 0 : values.Sum(d => Math.Pow(d - mean, 2)) / (n - 1);
        double CalcCentralMoment(int k) => values.Average(x => (x - mean).Pow(k));
        double skewness = CalcCentralMoment(3) / variance.Pow(1.5);
        double kurtosis = CalcCentralMoment(4) / variance.Pow(2);
        return new Moments(mean, variance, skewness, kurtosis);
    }

    public static Moments Create(Sample sample) => Create(sample.Values);
}