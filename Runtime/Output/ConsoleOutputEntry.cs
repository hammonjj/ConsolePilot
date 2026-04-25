using System;

namespace ConsolePilot.Output
{
    public readonly struct ConsoleOutputEntry
    {
        public ConsoleOutputEntry(string message, ConsoleOutputLevel level, DateTime timestamp)
        {
            Message = message ?? string.Empty;
            Level = level;
            Timestamp = timestamp;
        }

        public string Message { get; }

        public ConsoleOutputLevel Level { get; }

        public DateTime Timestamp { get; }

        public static ConsoleOutputEntry Create(string message, ConsoleOutputLevel level)
        {
            return new ConsoleOutputEntry(message, level, DateTime.Now);
        }
    }
}
