using System.Runtime.CompilerServices;

namespace Perfolizer.Helpers;

internal static class JitHelper
{
    // This is used to ensure clock methods are immediately promoted to tier 1 JIT, and are not re-jitted,
    // to avoid any tiered-JIT-related variance in time measurements.
    // AggressiveOptimization is not available in netstandard2.0, so just use the value casted to enum.
    internal const MethodImplOptions AggressiveOptimizationOption = (MethodImplOptions) 512;
}