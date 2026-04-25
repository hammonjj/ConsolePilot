using System.Collections.Generic;
using ConsolePilot.Core;

namespace ConsolePilot.Commands
{
    public interface IConsoleCommand
    {
        CommandDescriptor Descriptor { get; }

        CommandResult Execute(CommandContext context, IReadOnlyList<string> arguments);
    }
}
