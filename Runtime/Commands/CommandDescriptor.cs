using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsolePilot.Commands
{
    public sealed class CommandDescriptor
    {
        public CommandDescriptor(string name, string description, string usage, IEnumerable<string> aliases = null)
        {
            Name = name ?? string.Empty;
            Description = description ?? string.Empty;
            Usage = usage ?? string.Empty;
            Aliases = aliases == null
                ? Array.Empty<string>()
                : aliases.Where(alias => string.IsNullOrWhiteSpace(alias) == false)
                    .Select(alias => alias.Trim())
                    .ToArray();
        }

        public string Name { get; }

        public IReadOnlyList<string> Aliases { get; }

        public string Description { get; }

        public string Usage { get; }
    }
}
