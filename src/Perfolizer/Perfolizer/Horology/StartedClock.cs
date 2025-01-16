using JetBrains.Annotations;
using Perfolizer.Helpers;
using System.Runtime.CompilerServices;

namespace Perfolizer.Horology;

public readonly struct StartedClock
{
    private readonly IClock clock;
    private readonly long startTimestamp;

    [MethodImpl(JitHelper.AggressiveOptimizationOption)]
    public StartedClock(IClock clock, long startTimestamp)
    {
        this.clock = clock;
        this.startTimestamp = startTimestamp;
    }

    [MethodImpl(JitHelper.AggressiveOptimizationOption)]
    [Pure]
    public ClockSpan GetElapsed() => new ClockSpan(startTimestamp, clock.GetTimestamp(), clock.Frequency);

    public override string ToString() => $"StartedClock({clock.Title}, {startTimestamp} ticks)";
}