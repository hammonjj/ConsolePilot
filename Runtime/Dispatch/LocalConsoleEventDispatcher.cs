using System;
using System.Collections.Generic;

namespace ConsolePilot.Dispatch
{
    public sealed class LocalConsoleEventDispatcher : IConsoleEventDispatcher
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        public void Publish<TEvent>(TEvent eventData)
        {
            var eventType = typeof(TEvent);

            if (_subscribers.TryGetValue(eventType, out var callbacks) == false)
            {
                return;
            }

            foreach (var callback in callbacks.ToArray())
            {
                if (callback is Action<TEvent> action)
                {
                    action(eventData);
                }
            }
        }

        public IConsoleSubscription Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null)
            {
                return new DelegateConsoleSubscription(null);
            }

            var eventType = typeof(TEvent);

            if (_subscribers.TryGetValue(eventType, out var callbacks) == false)
            {
                callbacks = new List<Delegate>();
                _subscribers[eventType] = callbacks;
            }

            if (callbacks.Contains(handler) == false)
            {
                callbacks.Add(handler);
            }

            return new DelegateConsoleSubscription(() => Unsubscribe(handler));
        }

        private void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            var eventType = typeof(TEvent);

            if (_subscribers.TryGetValue(eventType, out var callbacks) == false)
            {
                return;
            }

            callbacks.Remove(handler);

            if (callbacks.Count == 0)
            {
                _subscribers.Remove(eventType);
            }
        }
    }
}
