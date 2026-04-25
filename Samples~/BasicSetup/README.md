# ConsolePilot Basic Setup

1. Install ConsolePilot as a local package through Unity Package Manager.
2. Create a GameObject named `ConsolePilot`.
3. Add `UIDocument`.
4. Add `ConsolePilotRuntime`.
5. Create a `ConsolePilotSettings` asset from `Create > ConsolePilot > Settings`.
6. Assign `Runtime/UI/UXML/ConsolePilot.uxml` and `Runtime/UI/USS/ConsolePilotTheme.uss` to the settings asset or directly to `ConsolePilotRuntime`.
7. Enter Play Mode and press backquote to open the console.

If the UXML asset is not assigned, `ConsolePilotRuntime` builds a minimal fallback UI at runtime.
