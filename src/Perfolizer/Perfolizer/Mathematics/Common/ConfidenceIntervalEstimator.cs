using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Mathematics.Common;

public class ConfidenceIntervalEstimator
{
    public double SampleSize { get; }
    public double Estimation { get; }
    public double StandardError { get; }
    private double DegreeOfFreedom => SampleSize - 1;

    public ConfidenceIntervalEstimator(double sampleSize, double estimation, double standardError)
    {
        SampleSize = sampleSize;
        Estimation = estimation;
        StandardError = standardError;
    }

    public ConfidenceInterval ConfidenceInterval(ConfidenceLevel confidenceLevel)
    {
        if (DegreeOfFreedom <= 0)
            return new ConfidenceInterval(Estimation, double.NaN, double.NaN, confidenceLevel);
        double margin = StandardError * ZLevel(confidenceLevel);
        return new ConfidenceInterval(Estimation, Estimation - margin, Estimation + margin, confidenceLevel);
    }

    public double ZLevel(ConfidenceLevel confidenceLevel)
    {
        double x = 1 - (1 - confidenceLevel) / 2;
        return new StudentDistribution(DegreeOfFreedom).Quantile(x);
    }
}