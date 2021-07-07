namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// The most common quantile estimator, also known as Type 7 (see [Hyndman 1996]).
    /// Consistent with many other statistical packages like R, Julia, NumPy, Excel (`PERCENTILE`, `PERCENTILE.INC`), Python (`inclusive`).
    /// <remarks>
    /// Hyndman, R. J. and Fan, Y. (1996) Sample quantiles in statistical packages, American Statistician 50, 361–365. doi: 10.2307/2684934.
    /// </remarks>
    /// </summary>
    public class SimpleQuantileEstimator : HyndmanFanQuantileEstimator
    {
        public static readonly IQuantileEstimator Instance = new SimpleQuantileEstimator();

        private SimpleQuantileEstimator() : base(HyndmanFanType.Type7)
        {
        }
    }
}