using JetBrains.Annotations;

namespace Perfolizer.Mathematics.OutlierDetection;

/// <summary>
/// Outlier detector based on fences.
/// Consider all values outside [LowerFence, UpperFence] as outliers.
/// </summary>
public abstract class FenceOutlierDetector : IOutlierDetector
{
    /// <summary>
    /// The lower fence of the outlier detector.
    /// All values that are less than the lower fence will be marked as outliers.
    /// </summary>
    [PublicAPI]
    public double LowerFence { get; protected set; }

    /// <summary>
    /// The upper fence of the outlier detector.
    /// All values that are greater than the upper fence will be marked as outliers.
    /// </summary>
    [PublicAPI]
    public double UpperFence { get; protected set; }

    public bool IsLowerOutlier(double x) => x < LowerFence;

    public bool IsUpperOutlier(double x) => x > UpperFence;

    protected void HandleEmptySample()
    {
        LowerFence = double.MinValue;
        UpperFence = double.MaxValue;
    }
}