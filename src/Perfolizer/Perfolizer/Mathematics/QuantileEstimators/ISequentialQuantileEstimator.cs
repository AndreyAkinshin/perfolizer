namespace Perfolizer.Mathematics.QuantileEstimators
{
    public interface ISequentialQuantileEstimator
    {
        void AddValue(double x);
        double GetQuantile();
    }
}