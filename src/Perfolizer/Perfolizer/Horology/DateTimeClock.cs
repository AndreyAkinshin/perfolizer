using Perfolizer.Helpers;
using System.Runtime.CompilerServices;

namespace Perfolizer.Horology;

internal class DateTimeClock : IClock
{
    private const long TicksPerSecond = (long) 10 * 1000 * 1000;

    public string Title => "DateTime";
    public bool IsAvailable => true;
    public Frequency Frequency
    {
        [MethodImpl(JitHelper.AggressiveOptimizationOption)]
        get => new Frequency(TicksPerSecond);
    }
    [MethodImpl(JitHelper.AggressiveOptimizationOption)]
    public long GetTimestamp() => DateTime.UtcNow.Ticks;
}