using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsolePilot.Commands
{
    public sealed class ConsoleCommandRegistry : IConsoleCommandRegistry
    {
        private readonly Dictionary<string, IConsoleCommand> _commandsByName = new Dictionary<string, IConsoleCommand>();
        private readonly Dictionary<string, IConsoleCommand> _commandsByLookup = new Dictionary<string, IConsoleCommand>();

        public bool Register(IConsoleCommand command, out string error)
        {
            error = string.Empty;

            if (command == null)
            {
                error = "Command cannot be null.";
                return false;
            }

            var descriptor = command.Descriptor;

            if (descriptor == null)
            {
                error = "Command descriptor cannot be null.";
                return false;
            }

            var normalizedName = Normalize(descriptor.Name);

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                error = "Command name cannot be empty.";
                return false;
            }

            var lookupKeys = BuildLookupKeys(descriptor);

            foreach (var key in lookupKeys)
            {
                if (_commandsByLookup.ContainsKey(key))
                {
                    error = $"Command name or alias '{key}' is already registered.";
                    return false;
                }
            }

            _commandsByName[normalizedName] = command;

            foreach (var key in lookupKeys)
            {
                _commandsByLookup[key] = command;
            }

            return true;
        }

        public bool Unregister(string nameOrAlias)
        {
            var lookupKey = Normalize(nameOrAlias);

            if (_commandsByLookup.TryGetValue(lookupKey, out var command) == false)
            {
                return false;
            }

            var descriptor = command.Descriptor;
            var nameKey = Normalize(descriptor.Name);
            _commandsByName.Remove(nameKey);

            foreach (var key in BuildLookupKeys(descriptor))
            {
                _commandsByLookup.Remove(key);
            }

            return true;
        }

        public bool TryGet(string nameOrAlias, out IConsoleCommand command)
        {
            return _commandsByLookup.TryGetValue(Normalize(nameOrAlias), out command);
        }

        public IReadOnlyList<IConsoleCommand> GetAll()
        {
            return _commandsByName.Values
                .OrderBy(command => command.Descriptor.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static IReadOnlyList<string> BuildLookupKeys(CommandDescriptor descriptor)
        {
            var keys = new List<string>();
            AddUniqueKey(keys, Normalize(descriptor.Name));

            foreach (var alias in descriptor.Aliases)
            {
                AddUniqueKey(keys, Normalize(alias));
            }

            return keys;
        }

        private static void AddUniqueKey(ICollection<string> keys, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (keys.Contains(key))
            {
                return;
            }

            keys.Add(key);
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().ToLowerInvariant();
        }
    }
}
