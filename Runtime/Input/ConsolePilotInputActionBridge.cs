using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsolePilot.Input
{
    public sealed class ConsolePilotInputActionBridge : MonoBehaviour
    {
        [SerializeField] private ConsolePilotRuntime _consolePilot;

        public void Open()
        {
            Target?.Open();
        }

        public void Close()
        {
            Target?.Close();
        }

        public void Toggle()
        {
            Target?.Toggle();
        }

        public void SetOpen(bool isOpen)
        {
            Target?.SetOpen(isOpen);
        }

        public void Open(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Open();
            }
        }

        public void Close(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Close();
            }
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Toggle();
            }
        }

        private ConsolePilotRuntime Target
        {
            get
            {
                if (_consolePilot == null)
                {
                    _consolePilot = GetComponent<ConsolePilotRuntime>();
                }

                return _consolePilot;
            }
        }
    }
}
