using UnityEngine;

namespace ConsolePilot.Samples.ExampleCommands
{
    public readonly struct SpawnDebugCubeRequested
    {
        public SpawnDebugCubeRequested(Vector3 position)
        {
            Position = position;
        }

        public Vector3 Position { get; }

        public override string ToString()
        {
            return $"SpawnDebugCubeRequested({Position})";
        }
    }
}
