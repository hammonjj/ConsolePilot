using System;

namespace ConsolePilot.Dispatch
{
    public sealed class DelegateConsoleSubscription : IConsoleSubscription
    {
        private Action _unsubscribe;

        public DelegateConsoleSubscription(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            var unsubscribe = _unsubscribe;
            _unsubscribe = null;
            unsubscribe?.Invoke();
        }
    }
}
