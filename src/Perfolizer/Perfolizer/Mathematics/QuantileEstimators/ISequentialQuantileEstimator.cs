using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface ISequentialQuantileEstimator
    {
        void Add(double value);
        double Quantile(Probability probability);
    }
}