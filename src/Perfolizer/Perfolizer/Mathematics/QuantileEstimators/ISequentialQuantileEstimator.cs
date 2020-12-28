namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface ISequentialQuantileEstimator
    {
        void Add(double value);
        double GetQuantile();
    }
}