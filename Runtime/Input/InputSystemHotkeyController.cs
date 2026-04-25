using System;
using ConsolePilot.Settings;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ConsolePilot.Input
{
    public sealed class InputSystemHotkeyController : IDisposable
    {
        private const string ToggleActionName = "ConsolePilotToggle";

        private InputAction _toggleAction;
        private bool _isEnabled;
        private bool _isCapturing;
        private Action<string> _captureCompleted;
        private Action _captureCanceled;

        public InputSystemHotkeyController(string bindingPath)
        {
            SetBinding(bindingPath);
        }

        public event Action ToggleRequested;

        public string BindingPath { get; private set; }

        public bool IsCapturing
        {
            get { return _isCapturing; }
        }

        public void Enable()
        {
            _isEnabled = true;
            _toggleAction?.Enable();
        }

        public void Disable()
        {
            _isEnabled = false;
            _toggleAction?.Disable();
        }

        public void SetBinding(string bindingPath)
        {
            BindingPath = string.IsNullOrWhiteSpace(bindingPath)
                ? ConsolePilotSettings.DefaultToggleBindingPath
                : bindingPath;

            var wasEnabled = _isEnabled;
            DisposeAction();

            _toggleAction = new InputAction(ToggleActionName, InputActionType.Button, BindingPath);
            _toggleAction.performed += OnTogglePerformed;

            if (wasEnabled && _isCapturing == false)
            {
                _toggleAction.Enable();
            }
        }

        public string GetBindingDisplayName()
        {
            return ToDisplayName(BindingPath);
        }

        public void BeginHotkeyCapture(Action<string> completed, Action canceled)
        {
            _isCapturing = true;
            _captureCompleted = completed;
            _captureCanceled = canceled;
            _toggleAction?.Disable();
        }

        public void CancelHotkeyCapture()
        {
            if (_isCapturing == false)
            {
                return;
            }

            _isCapturing = false;
            _captureCompleted = null;
            var canceled = _captureCanceled;
            _captureCanceled = null;
            canceled?.Invoke();

            if (_isEnabled)
            {
                _toggleAction?.Enable();
            }
        }

        public void Tick()
        {
            if (_isCapturing == false)
            {
                return;
            }

            var keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return;
            }

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                CancelHotkeyCapture();
                return;
            }

            foreach (var key in keyboard.allKeys)
            {
                if (key.wasPressedThisFrame == false)
                {
                    continue;
                }

                CompleteHotkeyCapture(ToBindingPath(key));
                return;
            }
        }

        public void Dispose()
        {
            _captureCompleted = null;
            _captureCanceled = null;
            DisposeAction();
        }

        public static string ToDisplayName(string bindingPath)
        {
            if (string.IsNullOrWhiteSpace(bindingPath))
            {
                return ToDisplayName(ConsolePilotSettings.DefaultToggleBindingPath);
            }

            var displayName = InputControlPath.ToHumanReadableString(
                bindingPath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            return string.IsNullOrWhiteSpace(displayName) ? bindingPath : displayName;
        }

        private static string ToBindingPath(KeyControl key)
        {
            return $"<Keyboard>/{key.name}";
        }

        private void CompleteHotkeyCapture(string bindingPath)
        {
            _isCapturing = false;

            var completed = _captureCompleted;
            _captureCompleted = null;
            _captureCanceled = null;

            SetBinding(bindingPath);
            completed?.Invoke(bindingPath);
        }

        private void OnTogglePerformed(InputAction.CallbackContext context)
        {
            if (_isCapturing)
            {
                return;
            }

            ToggleRequested?.Invoke();
        }

        private void DisposeAction()
        {
            if (_toggleAction == null)
            {
                return;
            }

            _toggleAction.performed -= OnTogglePerformed;
            _toggleAction.Disable();
            _toggleAction.Dispose();
            _toggleAction = null;
        }
    }
}
