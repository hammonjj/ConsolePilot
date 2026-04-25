using System;
using ConsolePilot.Commands;
using ConsolePilot.Input;
using ConsolePilot.Output;
using ConsolePilot.Settings;

namespace ConsolePilot.UI
{
    public sealed class ConsolePilotPresenter : IDisposable
    {
        private readonly ConsolePilotView _view;
        private readonly ConsoleCommandExecutor _executor;
        private readonly ConsoleOutputBuffer _output;
        private readonly InputSystemHotkeyController _hotkey;
        private readonly ConsoleRuntimeSettings _settings;

        public ConsolePilotPresenter(
            ConsolePilotView view,
            ConsoleCommandExecutor executor,
            ConsoleOutputBuffer output,
            InputSystemHotkeyController hotkey,
            ConsoleRuntimeSettings settings)
        {
            _view = view;
            _executor = executor;
            _output = output;
            _hotkey = hotkey;
            _settings = settings;
        }

        public bool IsOpen
        {
            get { return _view.IsOpen; }
        }

        public void Initialize()
        {
            _view.CommandSubmitted += OnCommandSubmitted;
            _view.CaptureHotkeyRequested += OnCaptureHotkeyRequested;
            _view.ResetHotkeyRequested += OnResetHotkeyRequested;
            _view.CloseRequested += Close;
            _output.EntryAdded += OnOutputEntryAdded;
            _output.Cleared += OnOutputCleared;
            _hotkey.ToggleRequested += Toggle;

            _view.SetHotkeyText(_hotkey.GetBindingDisplayName());
            _view.RenderEntries(_output.Entries);
            _view.SetOpen(_settings.OpenOnStart);
        }

        public void Tick()
        {
            _hotkey.Tick();
        }

        public void Open()
        {
            _view.SetOpen(true);
        }

        public void Close()
        {
            _view.SetOpen(false);
        }

        public void Toggle()
        {
            _view.SetOpen(_view.IsOpen == false);
        }

        public void Dispose()
        {
            _view.CommandSubmitted -= OnCommandSubmitted;
            _view.CaptureHotkeyRequested -= OnCaptureHotkeyRequested;
            _view.ResetHotkeyRequested -= OnResetHotkeyRequested;
            _view.CloseRequested -= Close;
            _output.EntryAdded -= OnOutputEntryAdded;
            _output.Cleared -= OnOutputCleared;
            _hotkey.ToggleRequested -= Toggle;
        }

        private void OnCommandSubmitted(string commandText)
        {
            _executor.Execute(commandText);
            _view.FocusInput();
        }

        private void OnOutputEntryAdded(ConsoleOutputEntry entry)
        {
            _view.AppendEntry(entry);
        }

        private void OnOutputCleared()
        {
            _view.ClearEntries();
        }

        private void OnCaptureHotkeyRequested()
        {
            _view.SetCaptureMode(true);
            _hotkey.BeginHotkeyCapture(OnHotkeyCaptured, OnHotkeyCaptureCanceled);
        }

        private void OnHotkeyCaptured(string bindingPath)
        {
            _settings.ToggleBindingPath = bindingPath;
            _view.SetCaptureMode(false);
            _view.SetHotkeyText(_hotkey.GetBindingDisplayName());
            _output.Write(ConsoleOutputEntry.Create($"Hotkey set to {_hotkey.GetBindingDisplayName()}.", ConsoleOutputLevel.System));
        }

        private void OnHotkeyCaptureCanceled()
        {
            _view.SetCaptureMode(false);
            _view.SetHotkeyText(_hotkey.GetBindingDisplayName());
        }

        private void OnResetHotkeyRequested()
        {
            _settings.ToggleBindingPath = ConsolePilotSettings.DefaultToggleBindingPath;
            _hotkey.SetBinding(_settings.ToggleBindingPath);
            _view.SetHotkeyText(_hotkey.GetBindingDisplayName());
            _output.Write(ConsoleOutputEntry.Create($"Hotkey reset to {_hotkey.GetBindingDisplayName()}.", ConsoleOutputLevel.System));
        }
    }
}
