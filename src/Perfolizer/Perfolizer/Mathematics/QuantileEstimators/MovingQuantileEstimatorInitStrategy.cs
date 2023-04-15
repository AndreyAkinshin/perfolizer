namespace Perfolizer.Mathematics.QuantileEstimators;

/// <summary>
/// Defines how a moving quantile estimator calculates the target quantile value
/// when the total number of elements is less than the window size
/// </summary>
public enum MovingQuantileEstimatorInitStrategy
{
    /// <summary>
    /// Approximate the target quantile.
    ///
    /// <example>
    /// windowSize = 5, k = 2 (the median)
    /// If the total number of elements equals 3, the median (k = 1) will be returned 
    /// </example> 
    /// </summary>
    QuantileApproximation,

    /// <summary>
    /// Return the requested order statistics
    ///
    /// <example>
    /// windowSize = 5, k = 2
    /// If the total number of elements equals 3, the largest element (k = 2) will be returned 
    /// </example> 
    /// </summary>
    OrderStatistics
}