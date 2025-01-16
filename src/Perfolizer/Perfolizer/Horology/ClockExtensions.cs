using Perfolizer.Helpers;
using System.Runtime.CompilerServices;

namespace Perfolizer.Horology;

public static class ClockExtensions
{
    public static TimeInterval GetResolution(this IClock clock) => clock.Frequency.ToResolution();

    [MethodImpl(JitHelper.AggressiveOptimizationOption)]
    public static StartedClock Start(this IClock clock) => new StartedClock(clock, clock.GetTimestamp());
}