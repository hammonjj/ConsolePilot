using System.Collections.Generic;
using ConsolePilot.Core;

namespace ConsolePilot.Commands.BuiltIn
{
    public sealed class EchoCommand : IConsoleCommand
    {
        public EchoCommand()
        {
            Descriptor = new CommandDescriptor(
                "echo",
                "Writes the provided text back to the console.",
                "echo <text>");
        }

        public CommandDescriptor Descriptor { get; }

        public CommandResult Execute(CommandContext context, IReadOnlyList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                return CommandResult.Info("Usage: echo <text>");
            }

            return CommandResult.Info(string.Join(" ", arguments));
        }
    }
}
