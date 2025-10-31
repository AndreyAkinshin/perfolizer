using Pragmastat;

namespace Perfolizer.Mathematics.QuantileEstimators;

public interface ISequentialQuantileEstimator
{
    void Add(double value);
    double Quantile(Probability probability);
}