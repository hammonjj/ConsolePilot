using System.Collections.Generic;
using System.Globalization;
using ConsolePilot.Commands;
using ConsolePilot.Core;
using UnityEngine;

namespace ConsolePilot.Samples.ExampleCommands
{
    public sealed class SpawnDebugCubeCommand : IConsoleCommand
    {
        public SpawnDebugCubeCommand()
        {
            Descriptor = new CommandDescriptor(
                "spawn_cube",
                "Publishes a request to spawn a debug cube.",
                "spawn_cube [x y z]",
                new[] { "cube" });
        }

        public CommandDescriptor Descriptor { get; }

        public CommandResult Execute(CommandContext context, IReadOnlyList<string> arguments)
        {
            if (TryParsePosition(arguments, out var position, out var error) == false)
            {
                return CommandResult.Fail(error);
            }

            context.Events.Publish(new SpawnDebugCubeRequested(position));
            return CommandResult.Ok($"Spawn debug cube requested at {position}.");
        }

        private static bool TryParsePosition(IReadOnlyList<string> arguments, out Vector3 position, out string error)
        {
            position = Vector3.zero;
            error = string.Empty;

            if (arguments.Count == 0)
            {
                return true;
            }

            if (arguments.Count != 3)
            {
                error = "Usage: spawn_cube [x y z]";
                return false;
            }

            if (TryParseFloat(arguments[0], out var x) &&
                TryParseFloat(arguments[1], out var y) &&
                TryParseFloat(arguments[2], out var z))
            {
                position = new Vector3(x, y, z);
                return true;
            }

            error = "Position arguments must be numeric values.";
            return false;
        }

        private static bool TryParseFloat(string value, out float result)
        {
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }
    }
}
