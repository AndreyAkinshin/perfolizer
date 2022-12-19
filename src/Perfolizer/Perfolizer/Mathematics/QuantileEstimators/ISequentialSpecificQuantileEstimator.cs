namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface ISequentialSpecificQuantileEstimator
    {
        void Add(double value);
        double Quantile();
    }
}