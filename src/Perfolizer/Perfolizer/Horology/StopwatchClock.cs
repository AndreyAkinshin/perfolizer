using Perfolizer.Helpers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Perfolizer.Horology;

internal class StopwatchClock : IClock
{
    public string Title => "Stopwatch";
    public bool IsAvailable => true;
    public Frequency Frequency
    {
        [MethodImpl(JitHelper.AggressiveOptimizationOption)]
        get => new Frequency(Stopwatch.Frequency);
    }
    [MethodImpl(JitHelper.AggressiveOptimizationOption)]
    public long GetTimestamp() => Stopwatch.GetTimestamp();
}