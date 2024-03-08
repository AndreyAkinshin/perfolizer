using JetBrains.Annotations;

namespace Perfolizer.Horology;

public readonly struct ClockSpan(long startTimestamp, long endTimestamp, Frequency frequency)
{
    [Pure] public double GetSeconds() => 1.0 * Max(0, endTimestamp - startTimestamp) / frequency;
    [Pure] public double GetNanoseconds() => GetSeconds() * TimeUnit.Second.BaseUnits;
    [Pure] public long GetDateTimeTicks() => (long)Round(GetSeconds() * TimeSpan.TicksPerSecond);
    [Pure] public TimeSpan GetTimeSpan() => new(GetDateTimeTicks());
    [Pure] public TimeInterval GetTimeValue() => new(GetNanoseconds());

    public override string ToString() =>
        $"ClockSpan({startTimestamp} ticks, {endTimestamp} ticks, {frequency.Hertz} Hz)";
}