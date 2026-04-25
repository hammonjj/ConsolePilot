using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsolePilot.Input
{
    public sealed class InputSystemConsoleTextController : IDisposable
    {
        private Keyboard _keyboard;
        private bool _isEnabled;
        private bool _isActive;
        private int _activatedFrame = -1;

        public event Action<char> CharacterTyped;

        public event Action BackspaceRequested;

        public event Action DeleteRequested;

        public event Action MoveCaretLeftRequested;

        public event Action MoveCaretRightRequested;

        public event Action SubmitRequested;

        public event Action CancelRequested;

        public void Enable()
        {
            _isEnabled = true;
            RefreshKeyboard();
        }

        public void Disable()
        {
            _isEnabled = false;
            UnsubscribeFromKeyboard();
        }

        public void SetActive(bool isActive)
        {
            if (_isActive == isActive)
            {
                return;
            }

            _isActive = isActive;

            if (_isActive)
            {
                _activatedFrame = Time.frameCount;
                RefreshKeyboard();
            }
        }

        public void Tick()
        {
            RefreshKeyboard();

            if (_isEnabled == false || _isActive == false || _keyboard == null)
            {
                return;
            }

            if (_keyboard.enterKey.wasPressedThisFrame || _keyboard.numpadEnterKey.wasPressedThisFrame)
            {
                SubmitRequested?.Invoke();
                return;
            }

            if (_keyboard.escapeKey.wasPressedThisFrame)
            {
                CancelRequested?.Invoke();
                return;
            }

            if (_keyboard.backspaceKey.wasPressedThisFrame)
            {
                BackspaceRequested?.Invoke();
                return;
            }

            if (_keyboard.deleteKey.wasPressedThisFrame)
            {
                DeleteRequested?.Invoke();
                return;
            }

            if (_keyboard.leftArrowKey.wasPressedThisFrame)
            {
                MoveCaretLeftRequested?.Invoke();
                return;
            }

            if (_keyboard.rightArrowKey.wasPressedThisFrame)
            {
                MoveCaretRightRequested?.Invoke();
            }
        }

        public void Dispose()
        {
            Disable();
        }

        private void RefreshKeyboard()
        {
            if (_isEnabled == false)
            {
                return;
            }

            if (_keyboard == Keyboard.current)
            {
                return;
            }

            UnsubscribeFromKeyboard();
            _keyboard = Keyboard.current;

            if (_keyboard != null)
            {
                _keyboard.onTextInput += OnTextInput;
            }
        }

        private void UnsubscribeFromKeyboard()
        {
            if (_keyboard == null)
            {
                return;
            }

            _keyboard.onTextInput -= OnTextInput;
            _keyboard = null;
        }

        private void OnTextInput(char character)
        {
            if (_isEnabled == false || _isActive == false)
            {
                return;
            }

            if (Time.frameCount == _activatedFrame || char.IsControl(character))
            {
                return;
            }

            CharacterTyped?.Invoke(character);
        }
    }
}
