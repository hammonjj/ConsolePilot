using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsolePilot.Core;

namespace ConsolePilot.Commands.BuiltIn
{
    public sealed class HelpCommand : IConsoleCommand
    {
        private readonly IConsoleCommandRegistry _commands;

        public HelpCommand(IConsoleCommandRegistry commands)
        {
            _commands = commands;
            Descriptor = new CommandDescriptor(
                "help",
                "Lists available commands or shows details for one command.",
                "help [command]",
                new[] { "?" });
        }

        public CommandDescriptor Descriptor { get; }

        public CommandResult Execute(CommandContext context, IReadOnlyList<string> arguments)
        {
            if (arguments.Count > 0)
            {
                return DescribeCommand(arguments[0]);
            }

            var builder = new StringBuilder();
            builder.AppendLine("Available commands:");

            foreach (var command in _commands.GetAll())
            {
                builder.Append("  ");
                builder.Append(command.Descriptor.Name);

                if (command.Descriptor.Aliases.Count > 0)
                {
                    builder.Append(" (");
                    builder.Append(string.Join(", ", command.Descriptor.Aliases));
                    builder.Append(")");
                }

                builder.Append(" - ");
                builder.AppendLine(command.Descriptor.Description);
            }

            builder.Append("Type 'help commandName' for details.");
            return CommandResult.Info(builder.ToString());
        }

        private CommandResult DescribeCommand(string commandName)
        {
            if (_commands.TryGet(commandName, out var command) == false)
            {
                return CommandResult.Fail($"Unknown command '{commandName}'.");
            }

            var descriptor = command.Descriptor;
            var builder = new StringBuilder();
            builder.AppendLine(descriptor.Name);
            builder.AppendLine(descriptor.Description);

            if (string.IsNullOrWhiteSpace(descriptor.Usage) == false)
            {
                builder.Append("Usage: ");
                builder.AppendLine(descriptor.Usage);
            }

            if (descriptor.Aliases.Any())
            {
                builder.Append("Aliases: ");
                builder.AppendLine(string.Join(", ", descriptor.Aliases));
            }

            return CommandResult.Info(builder.ToString().TrimEnd());
        }
    }
}
