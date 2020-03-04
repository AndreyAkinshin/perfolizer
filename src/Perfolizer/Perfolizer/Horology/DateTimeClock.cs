using System;

namespace Perfolizer.Horology
{
    internal class DateTimeClock : IClock
    {
        private const long TicksPerSecond = (long) 10 * 1000 * 1000;

        public string Title => "DateTime";
        public bool IsAvailable => true;
        public Frequency Frequency => new Frequency(TicksPerSecond);
        public long GetTimestamp() => DateTime.UtcNow.Ticks;
    }
}