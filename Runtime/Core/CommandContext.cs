using ConsolePilot.Dispatch;
using ConsolePilot.Output;
using ConsolePilot.Settings;

namespace ConsolePilot.Core
{
    public sealed class CommandContext
    {
        public CommandContext(IConsoleOutput output, IConsoleEventDispatcher events, ConsoleRuntimeSettings settings)
        {
            Output = output;
            Events = events;
            Settings = settings;
        }

        public IConsoleOutput Output { get; }

        public IConsoleEventDispatcher Events { get; }

        public ConsoleRuntimeSettings Settings { get; }
    }
}
