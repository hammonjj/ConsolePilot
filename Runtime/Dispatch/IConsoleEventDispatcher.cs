using System;

namespace ConsolePilot.Dispatch
{
    public interface IConsoleEventDispatcher
    {
        void Publish<TEvent>(TEvent eventData);

        IConsoleSubscription Subscribe<TEvent>(Action<TEvent> handler);
    }
}
