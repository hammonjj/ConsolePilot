# Changelog

## 0.1.2

- Fixed startup visibility so the console is hidden when `Open On Start` is disabled.

## 0.1.1

- Added package `.meta` files required for Git/UPM imports.
- Removed `.meta` files from hidden UPM folders.
- Added Input System-backed console text capture for reliable command entry.
- Exposed `ConsolePilotRuntime.OpenStateChanged` for gameplay input gating.

## 0.1.0

- Initial package scaffold.
- Added runtime command, output, settings, input, UI, and dispatch architecture.
- Added built-in `help`, `clear`, and `echo` commands.
- Added sample command and message bus adapter examples.
