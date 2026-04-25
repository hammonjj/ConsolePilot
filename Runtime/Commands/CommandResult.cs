using ConsolePilot.Output;

namespace ConsolePilot.Commands
{
    public readonly struct CommandResult
    {
        public CommandResult(bool success, string message, ConsoleOutputLevel outputLevel)
        {
            Success = success;
            Message = message;
            OutputLevel = outputLevel;
        }

        public bool Success { get; }

        public string Message { get; }

        public ConsoleOutputLevel OutputLevel { get; }

        public bool HasMessage
        {
            get { return string.IsNullOrWhiteSpace(Message) == false; }
        }

        public static CommandResult Ok(string message = null)
        {
            return new CommandResult(true, message, ConsoleOutputLevel.Success);
        }

        public static CommandResult Info(string message)
        {
            return new CommandResult(true, message, ConsoleOutputLevel.Info);
        }

        public static CommandResult Warning(string message)
        {
            return new CommandResult(true, message, ConsoleOutputLevel.Warning);
        }

        public static CommandResult Fail(string message)
        {
            return new CommandResult(false, message, ConsoleOutputLevel.Error);
        }
    }
}
