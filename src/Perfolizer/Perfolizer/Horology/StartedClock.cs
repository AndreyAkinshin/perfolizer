using JetBrains.Annotations;

namespace Perfolizer.Horology
{
    public readonly struct StartedClock
    {
        private readonly IClock clock;
        private readonly long startTimestamp;

        public StartedClock(IClock clock, long startTimestamp)
        {
            this.clock = clock;
            this.startTimestamp = startTimestamp;
        }

        [Pure] public ClockSpan GetElapsed() => new ClockSpan(startTimestamp, clock.GetTimestamp(), clock.Frequency);

        public override string ToString() => $"StartedClock({clock.Title}, {startTimestamp} ticks)";
    }
}