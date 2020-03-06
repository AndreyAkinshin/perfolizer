using System.Linq;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public static class QuantileEstimatorExtensions
    {
        public static double GetQuantile(this IQuantileEstimator estimator, [NotNull] double[] data, double quantile)
        {
            return estimator.GetQuantiles(data, new[] {quantile}).First();
        }
    }
}