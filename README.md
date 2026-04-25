# ConsolePilot

ConsolePilot is a reusable in-game debug console package for Unity. It provides a compact runtime console, explicit command registration, simple output history, runtime hotkey configuration, and an event dispatch abstraction that can be bridged into project-specific systems.

The package is intentionally independent from game code. It does not reference BitBox assemblies or any concrete project message bus.

## Installation

Use Unity Package Manager with a local package path:

1. Open `Window > Package Manager`.
2. Select `+ > Add package from disk`.
3. Select this package's `package.json`.
4. Ensure the Unity Input System package is enabled in the project.

ConsolePilot is built as its own assembly:

- Runtime assembly: `ConsolePilot.Runtime`
- Root namespace: `ConsolePilot`
- Required package dependency: `com.unity.inputsystem`

## Scene Setup

1. Create a GameObject named `ConsolePilot`.
2. Add a `UIDocument` component.
3. Add `ConsolePilotRuntime`.
4. Create a settings asset from `Create > ConsolePilot > Settings`.
5. Assign the settings asset to `ConsolePilotRuntime`.
6. Assign `Runtime/UI/UXML/ConsolePilot.uxml` and `Runtime/UI/USS/ConsolePilotTheme.uss` on the settings asset or directly on the runtime component.

If no UXML asset is assigned, `ConsolePilotRuntime` builds a minimal fallback UI at runtime.

## Opening the Console

The default toggle binding is:

```text
<Keyboard>/backquote
```

At runtime, press the backquote/tilde key to open or close the console. The console focuses the command input when opened.

ConsolePilot captures command text through the Unity Input System while the console is open. This keeps typing reliable even when UI Toolkit focus is inconsistent in Play Mode.

## Hotkey Configuration

The hotkey can be configured in two places:

- Default value: `ConsolePilotSettings.ToggleBindingPath`
- Runtime session value: the console settings panel

Open the console, select `Settings`, then select `Capture`. Press a new keyboard key to replace the toggle binding for the current session. Select `Reset` to restore backquote.

V1 does not persist runtime hotkey edits. Add an `IConsoleSettingsStore` later if persistent settings are needed.

## Adding Commands

Create a class that implements `IConsoleCommand`:

```csharp
using System.Collections.Generic;
using ConsolePilot.Commands;
using ConsolePilot.Core;

public sealed class GodModeCommand : IConsoleCommand
{
    public GodModeCommand()
    {
        Descriptor = new CommandDescriptor(
            "god_mode",
            "Toggles invulnerability.",
            "god_mode <on|off>",
            new[] { "god" });
    }

    public CommandDescriptor Descriptor { get; }

    public CommandResult Execute(CommandContext context, IReadOnlyList<string> arguments)
    {
        if (arguments.Count != 1)
        {
            return CommandResult.Fail("Usage: god_mode <on|off>");
        }

        return CommandResult.Ok($"God mode set to {arguments[0]}.");
    }
}
```

Register commands through a provider component:

```csharp
using ConsolePilot.Commands;
using UnityEngine;

public sealed class DebugCommandProvider : MonoBehaviour, IConsoleCommandProvider
{
    public void RegisterCommands(IConsoleCommandRegistry registry)
    {
        registry.Register(new GodModeCommand(), out _);
    }
}
```

Add the provider to the same GameObject as `ConsolePilotRuntime` or another component on that GameObject.

You can also register commands directly:

```csharp
consolePilot.RegisterCommand(new GodModeCommand(), out var error);
```

## Built-In Commands

- `help`: lists registered commands.
- `help commandName`: shows details for one command.
- `clear` / `cls`: clears output history.
- `echo`: prints parsed arguments back to the console.

## Command Parsing

The v1 parser is quote-aware and intentionally small:

```text
echo hello world
echo "hello world"
```

It supports whitespace-separated arguments and double-quoted arguments. It does not use reflection or automatic command discovery.

## Message Bus Integration

Commands receive a `CommandContext`. Use `context.Events` to publish typed events without coupling commands to scene behavior:

```csharp
context.Events.Publish(new SpawnDebugCubeRequested(position));
```

ConsolePilot provides:

- `IConsoleEventDispatcher`
- `LocalConsoleEventDispatcher`
- `IGenericMessageBus`
- `GenericMessageBusConsoleDispatcher`

For a generic message bus:

```csharp
var dispatcher = new GenericMessageBusConsoleDispatcher(myGenericBus);
consolePilot.SetEventDispatcher(dispatcher);
```

The generic bus must implement:

```csharp
public interface IGenericMessageBus
{
    void Publish<T>(T message);
    void Subscribe<T>(System.Action<T> callback);
    void Unsubscribe<T>(System.Action<T> callback);
}
```

## BitBox MessageBus Adapter

The BitBox adapter is provided as a sample template in:

```text
Samples~/MessageBusAdapters/BitBoxMessageBusConsoleDispatcher.cs.txt
```

Copy it into a game assembly, rename it to `.cs`, and reference the project-specific BitBox `MessageBus`. The runtime package itself does not reference BitBox.

## Blocking Gameplay Input

Unity gameplay `InputAction`s still receive keyboard input unless your game disables or gates them. ConsolePilot exposes `OpenStateChanged` so the consuming game can pause movement input while the console is visible:

```csharp
using ConsolePilot;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ConsolePilotGameplayInputGate : MonoBehaviour
{
    [SerializeField] private ConsolePilotRuntime _consolePilot;
    [SerializeField] private PlayerInput _playerInput;

    private void OnEnable()
    {
        _consolePilot.OpenStateChanged += OnConsoleOpenStateChanged;
    }

    private void OnDisable()
    {
        _consolePilot.OpenStateChanged -= OnConsoleOpenStateChanged;
    }

    private void OnConsoleOpenStateChanged(bool isOpen)
    {
        _playerInput.enabled = isOpen == false;
    }
}
```

If your project uses custom input services instead of `PlayerInput`, subscribe to the same event and disable the relevant gameplay action map there.

## Example Command

Import the `Example Commands` sample. It includes:

- `SpawnDebugCubeCommand`
- `SpawnDebugCubeRequested`
- `SpawnDebugCubeCommandProvider`
- `SpawnDebugCubeListener`

Run:

```text
spawn_cube
spawn_cube 1 2 3
```

The command publishes a typed request. The listener owns the scene-specific cube spawning behavior.

## Architecture

`ConsolePilotRuntime` is the MonoBehaviour composition root. It creates:

- command registry
- command parser
- command executor
- output buffer
- event dispatcher
- input hotkey controller
- UI Toolkit view/presenter

Most behavior lives in plain C# classes so it can be tested without scenes.

## Roadmap

Future features are tracked in `Documentation~/Roadmap.md`. Likely additions:

- autocomplete
- command history navigation
- searchable logs
- log filtering
- categories and tags
- runtime object inspection
- debug variables
- persistent settings
- custom skins/themes
- UPM publishing polish
- broader test coverage
- sample scene
