using System;

namespace ConsolePilot.Dispatch
{
    public sealed class GenericMessageBusConsoleDispatcher : IConsoleEventDispatcher
    {
        private readonly IGenericMessageBus _messageBus;

        public GenericMessageBusConsoleDispatcher(IGenericMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public void Publish<TEvent>(TEvent eventData)
        {
            _messageBus?.Publish(eventData);
        }

        public IConsoleSubscription Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (_messageBus == null || handler == null)
            {
                return new DelegateConsoleSubscription(null);
            }

            _messageBus.Subscribe(handler);
            return new DelegateConsoleSubscription(() => _messageBus.Unsubscribe(handler));
        }
    }
}
