using System.Collections.Generic;
using ConsolePilot.Core;

namespace ConsolePilot.Commands.BuiltIn
{
    public sealed class ClearCommand : IConsoleCommand
    {
        public ClearCommand()
        {
            Descriptor = new CommandDescriptor(
                "clear",
                "Clears the console output history.",
                "clear",
                new[] { "cls" });
        }

        public CommandDescriptor Descriptor { get; }

        public CommandResult Execute(CommandContext context, IReadOnlyList<string> arguments)
        {
            context.Output.Clear();
            return CommandResult.Ok();
        }
    }
}
