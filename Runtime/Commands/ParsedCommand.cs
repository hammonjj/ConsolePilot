using System;
using System.Collections.Generic;

namespace ConsolePilot.Commands
{
    public readonly struct ParsedCommand
    {
        public ParsedCommand(string name, IReadOnlyList<string> arguments)
        {
            Name = name ?? string.Empty;
            Arguments = arguments ?? Array.Empty<string>();
        }

        public string Name { get; }

        public IReadOnlyList<string> Arguments { get; }
    }
}
