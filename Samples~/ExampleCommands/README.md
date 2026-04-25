# ConsolePilot Example Commands

This sample adds a `spawn_cube` command.

1. Add `ConsolePilotRuntime` to a scene.
2. Add `SpawnDebugCubeCommandProvider` to the same GameObject or another scene GameObject.
3. Add `SpawnDebugCubeListener` to a scene GameObject.
4. Enter `spawn_cube` or `spawn_cube 1 2 3` in the console.

The command publishes a typed `SpawnDebugCubeRequested` event through `CommandContext.Events`. The listener subscribes through the runtime dispatcher and owns the scene-specific behavior.
