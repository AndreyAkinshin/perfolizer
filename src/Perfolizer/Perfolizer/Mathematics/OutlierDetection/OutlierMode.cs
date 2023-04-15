﻿namespace Perfolizer.Mathematics.OutlierDetection;

/// <summary>
/// The enum is design to remove some outliers from the distribution.
/// </summary>
public enum OutlierMode
{
    /// <summary>
    /// Don't remove outliers.
    /// </summary>
    DontRemove,

    /// <summary>
    /// Remove only upper outliers (which is bigger than upperFence).
    /// </summary>
    RemoveUpper,

    /// <summary>
    /// Remove only lower outliers (which is smaller than lowerFence).
    /// </summary>
    RemoveLower,

    /// <summary>
    /// Remove all outliers.
    /// </summary>
    RemoveAll
}