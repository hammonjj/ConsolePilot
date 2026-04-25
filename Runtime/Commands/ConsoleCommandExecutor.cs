using System;
using ConsolePilot.Core;
using ConsolePilot.Dispatch;
using ConsolePilot.Output;
using ConsolePilot.Settings;

namespace ConsolePilot.Commands
{
    public sealed class ConsoleCommandExecutor
    {
        private readonly IConsoleCommandParser _parser;
        private readonly IConsoleCommandRegistry _commands;
        private readonly IConsoleOutput _output;
        private readonly ConsoleRuntimeSettings _settings;
        private IConsoleEventDispatcher _events;

        public ConsoleCommandExecutor(
            IConsoleCommandParser parser,
            IConsoleCommandRegistry commands,
            IConsoleOutput output,
            IConsoleEventDispatcher events,
            ConsoleRuntimeSettings settings)
        {
            _parser = parser;
            _commands = commands;
            _output = output;
            _events = events;
            _settings = settings;
        }

        public void SetEventDispatcher(IConsoleEventDispatcher events)
        {
            _events = events;
        }

        public CommandResult Execute(string input)
        {
            var parseResult = _parser.Parse(input);

            if (parseResult.IsEmpty)
            {
                return CommandResult.Ok();
            }

            _output.Write(ConsoleOutputEntry.Create($"> {input}", ConsoleOutputLevel.System));

            if (parseResult.Success == false)
            {
                var parseError = CommandResult.Fail(parseResult.Error);
                WriteResult(parseError);
                return parseError;
            }

            if (_commands.TryGet(parseResult.Command.Name, out var command) == false)
            {
                var unknownCommand = CommandResult.Fail($"Unknown command '{parseResult.Command.Name}'. Type 'help' for a command list.");
                WriteResult(unknownCommand);
                return unknownCommand;
            }

            try
            {
                var context = new CommandContext(_output, _events, _settings);
                var result = command.Execute(context, parseResult.Command.Arguments);
                WriteResult(result);
                return result;
            }
            catch (Exception exception)
            {
                var exceptionResult = CommandResult.Fail($"Command '{parseResult.Command.Name}' failed: {exception.Message}");
                WriteResult(exceptionResult);
                return exceptionResult;
            }
        }

        private void WriteResult(CommandResult result)
        {
            if (result.HasMessage == false)
            {
                return;
            }

            _output.Write(ConsoleOutputEntry.Create(result.Message, result.OutputLevel));
        }
    }
}
