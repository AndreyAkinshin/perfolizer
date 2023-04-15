using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators;

public class GreenwaldKhannaSpecificQuantileEstimator : ISequentialSpecificQuantileEstimator
{
    private readonly GreenwaldKhannaQuantileEstimator estimator;
    private readonly Probability probability;

    public GreenwaldKhannaSpecificQuantileEstimator(Probability probability, double epsilon)
    {
        estimator = new GreenwaldKhannaQuantileEstimator(epsilon);
        this.probability = probability;
    }

    public void Add(double value)
    {
        estimator.Add(value);
    }

    public double Quantile()
    {
        return estimator.Quantile(probability);
    }
}