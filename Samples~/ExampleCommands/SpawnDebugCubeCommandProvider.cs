using ConsolePilot.Commands;
using UnityEngine;

namespace ConsolePilot.Samples.ExampleCommands
{
    public sealed class SpawnDebugCubeCommandProvider : MonoBehaviour, IConsoleCommandProvider
    {
        public void RegisterCommands(IConsoleCommandRegistry registry)
        {
            registry.Register(new SpawnDebugCubeCommand(), out _);
        }
    }
}
