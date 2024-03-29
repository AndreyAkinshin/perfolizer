using System.Diagnostics;

namespace Perfolizer.Horology;

internal class StopwatchClock : IClock
{
    public string Title => "Stopwatch";
    public bool IsAvailable => true;
    public Frequency Frequency => new Frequency(Stopwatch.Frequency);
    public long GetTimestamp() => Stopwatch.GetTimestamp();
}